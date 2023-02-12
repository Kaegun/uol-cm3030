using UnityEngine;

public interface IPickUp
{
	bool Despawns { get; set; }
	bool CanBePickedUp { get; set; }
	bool CanBeDropped { get; }
	bool PlayAnimation { get; }
	void OnPickUp(Transform pickupTransform);
	void OnDrop(bool destroy);
}
