using UnityEngine;

public class CharacterHealth : MonoBehaviour, IDamageable
{
    public int maxHP = 100;
    private int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(DamageData data)
    {
        currentHP -= data.damageAmount;
        Debug.Log($"{name} took {data.damageAmount} damage.");

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{name} died.");
    }
}
