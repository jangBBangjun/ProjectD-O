using UnityEngine;

public class CharacterCombat : MonoBehaviour, ICharacterCombat
{
    [Header("Skill Slots")]
    [SerializeField] private BaseSkill basic;
    [SerializeField] private BaseSkill skill1;
    [SerializeField] private BaseSkill skill2;
    [SerializeField] private BaseSkill ultimate;

    public void BasicAttack()
    {
        basic?.Execute();
    }

    public void Skill1()
    {
        skill1?.Execute();
    }

    public void Skill2()
    {
        skill2?.Execute();
    }

    public void Ultimate()
    {
        ultimate?.Execute();
    }
}
