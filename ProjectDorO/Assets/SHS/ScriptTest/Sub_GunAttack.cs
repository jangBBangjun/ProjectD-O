using UnityEngine;

public class Sub_GunAttack : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.name.Contains("Golem"))
            other.GetComponent<Sub_EnemyAI>()?.TakeDamage(2);
    }
}
