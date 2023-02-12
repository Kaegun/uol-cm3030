public interface IPossessable
{
    bool CanBePossessed { get; }
    bool PossessionCompleted { get; }
    void OnPossessionStarted(Spirit possessor);
    void WhileCompletingPossession(Spirit possessor);
    void OnPossessionCompleted(Spirit possessor);
    void OnDispossess();
}
