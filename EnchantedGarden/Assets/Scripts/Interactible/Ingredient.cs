using UnityEngine;

//	Logic in base
public class Ingredient : PickUpBase, IInteractor
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
				break;
			default:
				break;
		}
	}
}
