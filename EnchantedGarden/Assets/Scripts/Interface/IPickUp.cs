using UnityEngine;

public interface IPickUp
{
	bool CanBePickedUp { get; }
	bool CanBeDropped { get; }
	bool PlayAnimation { get; }
	void OnPickUp(Transform pickupTransform);
	void OnDrop(bool destroy);
}
