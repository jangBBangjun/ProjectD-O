using UnityEngine;

public class Damager : MonoBehaviour
{
    public DamageData damageData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            damageData.hitDirection = (other.transform.position - transform.position).normalized;
            target.TakeDamage(damageData);
        }
    }
}