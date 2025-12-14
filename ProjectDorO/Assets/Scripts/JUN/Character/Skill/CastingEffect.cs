using UnityEngine;
using System.Collections;

public class CastingEffect : MonoBehaviour
{
    private TestBasicSkillActive ownerSkill;
    private ParticleSystem ps;

    public void Init(TestBasicSkillActive skill)
    {
        ownerSkill = skill;
        ps = GetComponent<ParticleSystem>();

        StartCoroutine(WaitForEnd());
    }

    private IEnumerator WaitForEnd()
    {
        if (ps != null)
            yield return new WaitUntil(() => !ps.IsAlive(true));

        ownerSkill.ExecuteEffect();

        Destroy(gameObject);
    }
}
