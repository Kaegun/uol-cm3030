using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    GameObject GameObject { get; }
    bool CanInteractWith(IInteractable interactable);
    void OnInteract(IInteractable interactable);
    bool DestroyAfterInteract(IInteractable interactable);
}

