using UnityEngine;

public abstract class BaseSkill : MonoBehaviour, ISkill
{
    [SerializeField] protected float cooldown = 1f;
    protected float lastUsedTime = -999f;

    public float CooldownRemaining =>
        Mathf.Max(0, cooldown - (Time.time - lastUsedTime));

    public float CooldownDuration => cooldown;

    public bool CanExecute()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void Execute()
    {
        if (!CanExecute())
            return;

        lastUsedTime = Time.time;
        OnExecute();
    }

    protected abstract void OnExecute();
}
