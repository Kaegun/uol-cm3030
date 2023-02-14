using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    bool CanInteractWith(IInteractable interactable);
    void OnInteract(IInteractable interactable);
}

