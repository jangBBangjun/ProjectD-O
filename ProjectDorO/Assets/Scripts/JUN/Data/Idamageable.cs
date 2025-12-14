using UnityEngine;
public enum TeamType
{
    Player,
    Enemy,
    Neutral
}
public interface IDamageable
{
    TeamType Team { get; }
    void TakeDamage(DamageData data);
}
