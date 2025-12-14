using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;

    [Header("Timing")]
    public float cooldown = 1f;

    [Header("Visual")]
    public AnimationClip animation;
    public GameObject effectPrefab;

    [Header("Combat")]
    public float damage;
}
