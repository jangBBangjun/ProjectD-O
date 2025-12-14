using UnityEngine;

public abstract class BaseSkill : MonoBehaviour, ISkill
{
    [SerializeField] protected SkillData data;

    protected float lastUsedTime = -999f;

    public float CooldownRemaining =>
        Mathf.Max(0, data.cooldown - (Time.time - lastUsedTime));

    public float CooldownDuration => data.cooldown;

    public bool CanExecute()
    {
        return Time.time >= lastUsedTime + data.cooldown;
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
