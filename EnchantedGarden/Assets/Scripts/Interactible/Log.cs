using UnityEngine;

//	Log only needs to be a Type.
public class Log : PickUpBase, IInteractor
{
    public GameObject GameObject => gameObject;

    public bool CanInteractWith(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                return true;
            default:
                return false;
        }
    }

    public bool DestroyAfterInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                return true;
            default:
                return false;
        }
    }

    public void OnInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                //Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
