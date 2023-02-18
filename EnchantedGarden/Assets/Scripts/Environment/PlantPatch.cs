using UnityEngine;
using UnityEngine.Assertions;

public class PlantPatch : MonoBehaviour
{
	[SerializeField]
	private bool _containsPlant;

	[SerializeField]
	private Plant _plantPrefab;

	[SerializeField]
	private Transform _droppedPosition;

	[SerializeField]
	private Transform _plantedPosition;

	private Plant _plant;

	public bool ContainsPlant => _containsPlant;

	public void AddPlant(Plant plant)
	{
		_containsPlant = true;
		_plant = plant;
	}

	public void RemovePlant()
	{
		_containsPlant = false;
		_plant = null;
	}

	private void Start()
	{
		Assert.IsNotNull(_droppedPosition, Utility.AssertNotNullMessage(nameof(_droppedPosition)));
		Assert.IsNotNull(_plantedPosition, Utility.AssertNotNullMessage(nameof(_plantedPosition)));

		//	Instantiate a plant at the start of the level if flag is set
		if (_containsPlant)
		{
			//	Instantiate on the "planted" spot, set plant state to planted
			_plant = Instantiate(_plantPrefab, _plantedPosition.position, Quaternion.identity.RandomizeY());
			_plant.Replant(this);
		}
	}
}
