using UnityEngine;

public class SkillAnimationBridge : MonoBehaviour
{
    [Header("Skill Slots")]
    [SerializeField] private TestBasicSkillActive basic;
    [SerializeField] private TestBasicSkillActive skill1;
    [SerializeField] private TestBasicSkillActive skill2;
    [SerializeField] private TestBasicSkillActive ultimate;

    public void AnimEvent_Basic()
    {
        basic?.ExecuteEffect();
    }

    public void AnimEvent_Skill1()
    {
        skill1?.ExecuteEffect();
    }
    
    public void AnimEvent_Skill2()
    {
        skill2?.ExecuteEffect();
    }

    public void AnimEvent_Ultimate()
    {
        ultimate?.ExecuteEffect();
    }
}
