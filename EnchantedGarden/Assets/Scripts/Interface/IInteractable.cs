using UnityEngine;

public interface IInteractable
{
    Transform Transform { get; }
    GameObject GameObject { get; }
    bool CanInteractWith(IInteractor interactor);
    void OnInteractWith(IInteractor interactor);
    bool DestroyOnInteract(IInteractor interactor);
}