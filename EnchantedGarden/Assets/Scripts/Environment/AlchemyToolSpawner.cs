using UnityEngine;

public class AlchemyToolSpawner : MonoBehaviour
{
	[SerializeField]
	private StaticSpawnedObject _ingredient;

	[SerializeField]
	private StaticSpawnedObject _potion;

	[SerializeField]
	private StaticSpawnedObject _trickPlant;

	//	TODO: Spawn a couple on the table
	private const int _trickPlantRows = 2;
	private const int _trickPlantColumns = 2;

	//  Start is called before the first frame update
	private void Start()
	{
		StaticSpawnedObject.Valid(_trickPlant);
		StaticSpawnedObject.Valid(_ingredient);
		StaticSpawnedObject.Valid(_potion);

		_ingredient.Spawn();
		_trickPlant.Spawn();
		_potion.Spawn();
	}
}
