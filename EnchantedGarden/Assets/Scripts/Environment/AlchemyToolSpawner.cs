using UnityEngine;
using UnityEngine.Assertions;

public class AlchemyToolSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject _trickPlantPrefab;

	[SerializeField]
	private Transform _trickPlantPosition;

	[SerializeField]
	private GameObject _mixturePrefab;

	[SerializeField]
	private Transform _mixturePosition;

	private const float _trickPlantScale = 0.3f;
	//	TODO: Spawn a couple on the table
	private const int _trickPlantRows = 1;
	private const int _trickPlantColumns = 1;

	//  Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_trickPlantPrefab);
		Assert.IsNotNull(_mixturePrefab);
		Assert.IsNotNull(_trickPlantPosition);
		Assert.IsNotNull(_mixturePosition);

		SpawnAlchemyMixtures();
		SpawnTrickPlants();
	}

	private void SpawnTrickPlants()
	{
		//	TODO: Spawn a couple of shrunken plants around the spawn point
		//	Calculate offset to apply
		float offset = 1.0f;
		for (int i = 0; i < _trickPlantRows; i++)
		{
			//	TODO: Position the plants correctly
			for (int j = 0; j < _trickPlantColumns; j++)
			{
				var tp = Instantiate(_trickPlantPrefab, _trickPlantPosition);
				tp.transform.localScale *= _trickPlantScale;
				tp.transform.position += new Vector3(i * offset, 0, j * offset);
			}
		}
	}

	private void SpawnAlchemyMixtures()
	{
		Instantiate(_mixturePrefab, _mixturePosition);
	}
}
