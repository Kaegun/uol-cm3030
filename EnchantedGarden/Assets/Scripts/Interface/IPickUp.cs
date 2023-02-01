using UnityEngine;

public interface IPickUp
{
	bool CanBePickedUp { get; }
	bool CanBeDropped { get; }
	GameObject PickUpObject();
	void OnPickUp();
	void OnDrop();
}
