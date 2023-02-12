using UnityEngine;
using UnityEngine.Assertions;

public class IngredientSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _ingredientPrefabs;

	// Start is called before the first frame update
	void Start()
	{
		Assert.IsNotNull(_ingredientPrefabs);
		Assert.IsTrue(_ingredientPrefabs.Length > 0);

		Debug.Log("Why aint you spawning");
		Instantiate(_ingredientPrefabs[Random.Range(0, _ingredientPrefabs.Length - 1)], transform);
	}
}
