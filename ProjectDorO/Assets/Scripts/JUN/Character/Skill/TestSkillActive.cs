using UnityEngine;

public class TestSkillActive : BaseSkill
{
    protected override void OnExecute()
    {
        Animator animator = GetComponent<Animator>();
        if (animator && data.animation)
            animator.Play(data.animation.name);

        if (data.effectPrefab)
            Instantiate(
                data.effectPrefab,
                transform.position,
                transform.rotation
            );
    }
}
