using UnityEngine;

public enum SkillSlot
{
    Basic,
    Skill1,
    Skill2,
    Ultimate
}

public class TestBasicSkillActive : BaseSkill
{
    [Header("Debug / Inspector")]
    [SerializeField] private SkillSlot slot;
    [SerializeField] private string skillDisplayName = "Skill";

    public string SkillName => skillDisplayName;

    [SerializeField] private Transform cameraTransform;

    protected override void OnExecute()
    {
        Animator animator = GetComponent<Animator>();
        if (animator && data.animation)
        {
            animator.Play(data.animation.name);
        }
    }
    public void AnimEvent_ExecuteSkill()
    {
        SpawnEffect();
    }
    public void AnimEvent_EndSkill()
    {
        //애니메이션 종료 시점
    }
    private void SpawnEffect()
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

        Destroy(go, data.duration);
    }

    private void SpawnMelee()
    {
        Vector3 pos = transform.position + transform.TransformDirection(data.spawnOffset);

        Instantiate(
            data.effectPrefab,
            pos,
            transform.rotation
        );
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

            DamageData dmg = new DamageData
            {
                damageAmount = data.damage,
                attacker = gameObject,
                attackType = data.attackType,
                hitDirection = cameraTransform.forward
            };

            target.TakeDamage(dmg);
        }
    }
}

