using UnityEngine;

public enum SkillSlot
{
    Basic,
    Skill1,
    Skill2,
    Ultimate
}

public enum SkillExecutionPhase
{
    None,
    Casting,
    Executing
}

public class TestBasicSkillActive : BaseSkill
{
    [Header("Debug / Inspector")]
    [SerializeField] private SkillSlot slot;
    [SerializeField] private string skillDisplayName = "Skill";
    public string SkillName => skillDisplayName;

    [SerializeField] private Transform cameraTransform;

    private SkillExecutionPhase phase = SkillExecutionPhase.None;

    protected override void OnExecute()
    {
        Animator animator = GetComponent<Animator>();
        if (animator && data.animation)
        {
            animator.Play(data.animation.name);
        }
    }

    // 애니메이션 이벤트 / 입력에서 호출
    public void ExecuteEffect()
    {
        switch (phase)
        {
            case SkillExecutionPhase.None:
                if (data.useCasting && data.castingEffectPrefab)
                {
                    phase = SkillExecutionPhase.Casting;
                    SpawnCastingEffect();
                }
                else
                {
                    phase = SkillExecutionPhase.Executing;
                    SpawnEffect();
                    phase = SkillExecutionPhase.None;
                }
                break;

            case SkillExecutionPhase.Casting:
                phase = SkillExecutionPhase.Executing;
                SpawnEffect();
                phase = SkillExecutionPhase.None;
                break;
        }
    }

    #region Casting
    private void SpawnCastingEffect()
    {
        GameObject go = Instantiate(
            data.castingEffectPrefab,
            transform.position + transform.TransformDirection(data.spawnOffset),
            transform.rotation
        );

        if (go.TryGetComponent(out CastingEffect casting))
        {
            casting.Init(this); // 캐스팅 종료 시 ExecuteEffect 다시 호출
        }
    }
    #endregion

    #region Spawn Effect
    protected override void SpawnEffect()
    {
        if (!data.effectPrefab)
            return;

        switch (data.spawnType)
        {
            case SkillSpawnType.Projectile:
                SpawnProjectile();
                break;

            case SkillSpawnType.Area:
                SpawnArea();
                break;

            case SkillSpawnType.Melee:
                SpawnMelee();
                break;

            case SkillSpawnType.Targeting:
                SpawnTargeting();
                break;

            case SkillSpawnType.Just:
                SpawnJust();
                break;
        }
    }

    private void SpawnProjectile()
    {
        Vector3 dir = cameraTransform.forward;
        Vector3 pos = transform.position + transform.TransformDirection(data.spawnOffset);

        GameObject go = Instantiate(
            data.effectPrefab,
            pos,
            Quaternion.LookRotation(dir)
        );

        InitDamager(go);

        if (go.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = dir * data.projectileSpeed;
        }
    }

    private void SpawnArea()
    {
        Vector3 pos =
            transform.position +
            cameraTransform.forward * data.forwardOffset +
            transform.TransformDirection(data.spawnOffset);

        GameObject go = Instantiate(
            data.effectPrefab,
            pos,
            Quaternion.identity
        );

        InitDamager(go);

        Destroy(go, data.duration);
    }

    private void SpawnMelee()
    {
        Vector3 pos = transform.position + transform.TransformDirection(data.spawnOffset);

        GameObject go = Instantiate(
            data.effectPrefab,
            pos,
            transform.rotation
        );

        InitDamager(go);
    }

    private void SpawnTargeting()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            cameraTransform.position,
            1.5f,
            cameraTransform.forward,
            data.range
        );

        foreach (var hit in hits)
        {
            if (!hit.collider.TryGetComponent(out IDamageable target))
                continue;

            DamageData dmg = CreateDamageData(cameraTransform.forward);
            target.TakeDamage(dmg);
        }
    }

    private void SpawnJust()
    {
        GameObject go = Instantiate(
            data.effectPrefab,
            transform.position + transform.TransformDirection(data.spawnOffset),
            transform.rotation
        );

        InitDamager(go);
    }
    #endregion

    #region Damager
    private void InitDamager(GameObject effect)
    {
        if (!effect.TryGetComponent(out Damager damager))
            return;

        DamageData dmg = CreateDamageData(
            (effect.transform.position - transform.position).normalized
        );

        damager.Init(dmg);
    }

    private DamageData CreateDamageData(Vector3 dir)
    {
        return new DamageData
        {
            damageAmount = data.damage,
            attacker = gameObject,
            attackType = data.attackType,
            hitDirection = dir
        };
    }
    #endregion
}
