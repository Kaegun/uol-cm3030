using System;
using UnityEngine;

[Obsolete("Don't think we need this anymore")]
public class PickUpStore : MonoBehaviour, IPickUp
{
	[SerializeField]
	private GameObject _pickUp;

	public bool CanBeDropped => true;

	public bool CanBePickedUp => true;

	public void OnDrop() { }

	public void OnPickUp(Transform _) { }

	public GameObject PickUpObject()
	{
		return Instantiate(_pickUp, transform.position, Quaternion.identity);
	}
}
