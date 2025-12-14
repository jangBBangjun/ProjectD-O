public interface ISkill
{
    bool CanExecute();
    void Execute();
    float CooldownRemaining { get; }
    float CooldownDuration { get; }
}
