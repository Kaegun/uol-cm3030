using UnityEngine;

public interface IInteractable { }

public interface IInteractable<T> : IInteractable
{
    Transform Transform { get; }
    bool CanInteractWith<I>(I interactor);
    void OnInteractWith<I>(I interactor);
    //void OnPlayerInteract(PlayerInteractionController player);
}
