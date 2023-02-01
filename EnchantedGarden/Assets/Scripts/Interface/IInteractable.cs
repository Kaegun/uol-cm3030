public interface IInteractable
{
	bool CanBeInteractedWith { get; }
	void OnPlayerInteract(PlayerInteractionController player);
}
