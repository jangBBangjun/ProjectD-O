using UnityEngine;

public class MijeongCombat : MonoBehaviour, ICharacterCombat
{
    public ISkill basic;
    public ISkill skill1;
    public ISkill skill2;
    public ISkill ultimate;

    public void BasicAttack() => basic?.Execute();
    public void Skill1() => skill1?.Execute();
    public void Skill2() => skill2?.Execute();
    public void Ultimate() => ultimate?.Execute();

}
