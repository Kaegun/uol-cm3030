using UnityEngine;
using UnityEngine.Assertions;

//	Logic in base
public class Ingredient : PickUpBase
{
	[SerializeField]
	private GameObject[] _ingredientPrefabs;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_ingredientPrefabs);
		Assert.IsTrue(_ingredientPrefabs.Length > 0);

		Debug.Log("Where are the ingredients?");
		Instantiate(_ingredientPrefabs[Random.Range(0, _ingredientPrefabs.Length - 1)], transform);
	}
}
