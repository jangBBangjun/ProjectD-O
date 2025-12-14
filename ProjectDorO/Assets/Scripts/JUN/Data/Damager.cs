using System.Collections.Generic;
using UnityEngine;

public enum HitPolicy
{
    Once,       // 한 번만 맞음
    Cooldown    // 일정 시간마다 맞음
}


public class Damager : MonoBehaviour
{
    private DamageData damageData;

    [Header("Hit Settings")]
    [SerializeField] private HitPolicy hitPolicy = HitPolicy.Once;
    [SerializeField] private float hitInterval = 0.5f;

    private HashSet<IDamageable> hitTargets = new();
    private Dictionary<IDamageable, float> lastHitTime = new();

    public void Init(DamageData data)
    {
        damageData = data;
        hitTargets.Clear();
        lastHitTime.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable target))
            return;

        TryHit(target);
    }

    private void OnTriggerStay(Collider other)
    {
        if (hitPolicy != HitPolicy.Cooldown)
            return;

        if (!other.TryGetComponent(out IDamageable target))
            return;

        TryHit(target);
    }

    private void TryHit(IDamageable target)
    {
        switch (hitPolicy)
        {
            case HitPolicy.Once:
                if (hitTargets.Contains(target))
                    return;

                hitTargets.Add(target);
                DealDamage(target);
                break;

            case HitPolicy.Cooldown:
                float lastTime = lastHitTime.GetValueOrDefault(target, -999f);
                if (Time.time < lastTime + hitInterval)
                    return;

                lastHitTime[target] = Time.time;
                DealDamage(target);
                break;
        }
    }

    private void DealDamage(IDamageable target)
    {
        damageData.hitDirection =
            (target as Component).transform.position - transform.position;

        target.TakeDamage(damageData);
    }
}

