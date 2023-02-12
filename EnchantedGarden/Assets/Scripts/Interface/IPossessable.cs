using UnityEngine;

public interface IPossessable
{
    bool CanBePossessed { get; }
    bool PossessionCompleted { get; }
    Transform Transform { get; }
    GameObject GameObject { get; }
    void OnPossessionStarted(Spirit possessor);
    void WhileCompletingPossession(Spirit possessor);
    void OnPossessionCompleted(Spirit possessor);
    void OnDispossess();
}
