using UnityEngine;

public class PickUpStore : MonoBehaviour, IPickUp
{
	[SerializeField]
	private GameObject _pickUp;

	public bool CanBeDropped => true;

	public bool CanBePickedUp => true;

	public void OnDrop() { }

	public void OnPickUp() { }

	public GameObject PickUpObject()
	{
		return Instantiate(_pickUp, transform.position, Quaternion.identity);
	}
}
