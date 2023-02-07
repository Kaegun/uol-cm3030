using UnityEngine;

public interface IPickUp
{
	bool CanBePickedUp { get; }
	bool CanBeDropped { get; }
	void OnPickUp(Transform pickupTransform);
	void OnDrop();
}
