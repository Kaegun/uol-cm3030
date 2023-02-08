using UnityEngine;
using UnityEngine.Assertions;

/*
 * TODO: Spawn timers?
 * Limit spawn count
 */

public abstract class PickUpSpawnerBase : MonoBehaviour
{
	[SerializeField]
	private GameObject _spawnedPrefab;

	public IPickUp SpawnPickUp()
	{
		var go = Instantiate(_spawnedPrefab);
		var result = go.GetComponent<IPickUp>();
		Assert.IsNotNull(result);
		return result;
	}
}
