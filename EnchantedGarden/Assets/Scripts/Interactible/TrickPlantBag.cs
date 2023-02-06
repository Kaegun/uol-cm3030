using UnityEngine;

public class TrickPlantBag : MonoBehaviour, IPickUp
{
	[SerializeField]
	private TrickPlant _trickPlant;

	[SerializeField]
	private Transform _spawnTransform;
	public bool CanBePickedUp => true;

	public bool CanBeDropped => true;

		public GameObject PickUpObject()
	{
		return Instantiate(_trickPlant, _spawnTransform.position, Quaternion.identity).gameObject;
	}

	public void OnPickUp() { }

	public void OnDrop() { }
}
