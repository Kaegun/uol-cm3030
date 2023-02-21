//	Code is in base
using UnityEngine;

public class Shovel : PickUpBase, IInteractor
{
    public GameObject GameObject => gameObject;

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

    }

    public bool DestroyAfterInteract(IInteractable interactable)
    {
        return false;
    }
}
