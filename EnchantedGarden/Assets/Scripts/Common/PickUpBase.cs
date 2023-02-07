using UnityEngine;

public abstract class PickUpBase : MonoBehaviour, IPickUp
{
	[Header("Pickup Transform")]
	[SerializeField]
	protected Vector3 _adjustmentPosition;

	[SerializeField]
	protected Vector3 _adjustmentRotation;

	[SerializeField]
	protected float _adjustmentScaleFactor = 100f;

	public bool CanBeDropped => _held;

	public bool CanBePickedUp => !_held;

	protected bool _held;

	public virtual void OnDrop()
	{
		_held = false;
		//	Set parent to null if its being held
		transform.SetParent(null);

		//	Randomize Y rotation of dropped object - could be parameterized
		transform.SetPositionAndRotation(new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity.RandomizeY());
	}

	public virtual void OnPickUp(Transform pickupTransform)
	{
		_held = true;

		transform.SetParent(pickupTransform, false);

		transform.localPosition = Vector3.zero + _adjustmentPosition;
		transform.localRotation = Quaternion.Euler(_adjustmentRotation.x, _adjustmentRotation.y, _adjustmentRotation.z);
		transform.localScale *= _adjustmentScaleFactor;
	}
}
