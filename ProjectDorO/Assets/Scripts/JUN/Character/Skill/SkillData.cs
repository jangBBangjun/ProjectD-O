using UnityEngine;
public enum SkillSpawnType
{
    Projectile,
    Area,
    Melee,
    Targeting,
    Just
}

[CreateAssetMenu(menuName = "Skill/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Casting")]
    public bool useCasting;
    public GameObject castingEffectPrefab;

    [Header("Visual")]
    public AnimationClip animation;
    public GameObject effectPrefab;

    [Header("Spawn")]
    public SkillSpawnType spawnType;
    public Vector3 spawnOffset;
    public float forwardOffset;

    [Header("Projectile")]
    public float projectileSpeed;

    [Header("Area")]
    public float duration;

    [Header("Targeting")]
    public float range;

    [Header("Damage")]
    public int damage;
    public AttackType attackType;

    [Header("Cooldown")]
    public float cooldown;
}
