using UnityEngine;

public interface IInteractable
{
    Transform Transform { get; }
    bool CanInteractWith(IInteractor interactor);
    void OnInteractWith(IInteractor interactor);
}