using UnityEngine;

public interface IInteractable { }

// Implement IInteractable<T> on anything you want T to be able to interact with
public interface IInteractable<T> : IInteractable
{
    Transform Transform { get; }
    bool CanInteractWith<I>(I interactor);
    void OnInteractWith<I>(I interactor);
    //void OnPlayerInteract(PlayerInteractionController player);
}

// TODO: Create IInteractor interface for objects that interact with IInteractables?
