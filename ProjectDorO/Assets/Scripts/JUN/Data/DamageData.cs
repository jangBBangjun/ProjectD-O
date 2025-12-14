using UnityEngine;

public enum AttackType
{
    Basic,
    Skill1,
    Skill2,
    Ultimate,
    Magic,
    Ranged
}

[System.Serializable]
public struct DamageData
{
    public int damageAmount;
    public GameObject attacker;
    public Vector3 hitDirection;
    public AttackType attackType;
}
