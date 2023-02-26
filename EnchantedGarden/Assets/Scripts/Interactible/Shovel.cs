using System;
using UnityEngine;

public class Shovel : PickUpBase, IInteractor, IEventPublisher
{
    public GameObject GameObject => gameObject;

    public static event EventHandler<Shovel> DigEvent;

    public bool CanInteractWith(IInteractable interactable)
    {
        switch (interactable)
        {
            case Plant _:
                return true;
            default:
                return false;
        }
    }

    public void OnInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Plant _:
                this.ExecuteEvent(DigEvent, this);
                break;
            default:
                break;
        }
    }

    public bool DestroyAfterInteract(IInteractable interactable)
    {
        return false;
    }
}
