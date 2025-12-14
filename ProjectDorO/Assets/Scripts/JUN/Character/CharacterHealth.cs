using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [Header("Team")]
    [SerializeField] private TeamType team;
    public TeamType Team => team;

    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDead;

    private void Awake()
    {
        currentHP = maxHP;
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(DamageData data)
    {
        if (!CanBeDamagedBy(data.attacker))
            return;

        currentHP -= data.damageAmount;
        currentHP = Mathf.Max(currentHP, 0);

        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
            Die();
    }

    private bool CanBeDamagedBy(GameObject attacker)
    {
        if (attacker == null) return false;
        if (!attacker.TryGetComponent(out IDamageable attackerDmg)) return false;
        return attackerDmg.Team != Team;
    }

    private void Die()
    {
        OnDead?.Invoke();
        // 애니메이션, 비활성화, 리스폰 등
    }
}
