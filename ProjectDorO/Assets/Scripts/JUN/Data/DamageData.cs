using UnityEngine;

[System.Serializable]
public struct DamageData
{
    public int damageAmount;
    public GameObject attacker;
    public Vector3 hitDirection;
    public string attackType; // ¿¹: "Normal", "Skill1", "Magic", etc.
}