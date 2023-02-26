public interface IInteractor
{
	bool CanInteractWith(IInteractable interactable);
	void OnInteract(IInteractable interactable);
	bool DestroyAfterInteract(IInteractable interactable);
	void DestroyInteractor();
}

