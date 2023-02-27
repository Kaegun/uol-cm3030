//	Log only needs to be a Type.
public class Log : PickUpBase, IInteractor
{
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

	public void DestroyInteractor()
	{
		Destroy(gameObject);
	}
}
