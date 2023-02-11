using UnityEngine;

//	TODO: Not sure we need this
public class TrickPlantBag : MonoBehaviour, IPickUp
{
	[SerializeField]
	private TrickPlant _trickPlant;

	[SerializeField]
	private Transform _spawnTransform;

	public bool CanBePickedUp => true;

	public bool CanBeDropped => true;
	
	public bool PlayAnimation => false;


	public GameObject PickUpObject()
	{
		return Instantiate(_trickPlant, _spawnTransform.position, Quaternion.identity).gameObject;
	}

	public void OnPickUp(Transform _) { }

	public void OnDrop(bool despawn = false) { }
}
