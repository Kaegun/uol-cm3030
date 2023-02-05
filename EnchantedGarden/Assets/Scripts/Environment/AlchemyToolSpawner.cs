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

	//  Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_trickPlantPrefab);
		Assert.IsNotNull(_mixturePrefab);
		Assert.IsNotNull(_trickPlantPosition);
		Assert.IsNotNull(_mixturePosition);

		Instantiate(_trickPlantPrefab, _trickPlantPosition);
		Instantiate(_mixturePrefab, _mixturePosition);
	}
}
