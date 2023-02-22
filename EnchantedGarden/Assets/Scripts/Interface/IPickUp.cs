using UnityEngine;

public interface IPickUp
{
	Transform Transform { get; }
	bool Despawns { get; set; }
	bool Despawned { get; }
	bool CanBePickedUp { get; set; }
	bool CanBeDropped { get; }
	bool PlayAnimation { get; }
	Vector3 IndicatorPostion { get; }
	Sprite CarryIcon { get; }
	Sprite CarryIconSecondary { get; }
	Color CarryIconBaseColor { get; }
	Color CarryIconSecondaryColor { get; }
	void OnPickUp(Transform pickupTransform);
	void OnDrop(bool destroy);
}
