using UnityEngine;

public interface IPickUp
{
	//	TODO: These can be properties?
	bool CanBePickedUp();
	bool CanBeDropped();
	GameObject PickUpObject();
	void OnPickUp();
	void OnDrop();
}
