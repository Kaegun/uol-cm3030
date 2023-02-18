using UnityEngine;
using UnityEngine.Assertions;

/*
 * TODO: Spawn timers?
 * Limit spawn count
 */

public abstract class PickUpSpawnerBase : MonoBehaviour
{
	[Header("Pickup Prefab")]
	[SerializeField]
	private PickUpBase _spawnedPrefab;

	[Header("Pickup Indicator Position")]
	[SerializeField]
	private Vector3 _pickUpIndicatorAdjustment;

	public Vector3 IndicatorPosition => transform.position + _pickUpIndicatorAdjustment;

	public IPickUp SpawnPickUp()
	{
		var pickUp = Instantiate(_spawnedPrefab);
		Assert.IsNotNull(pickUp, Utility.AssertNotNullMessage(nameof(pickUp)));
		return pickUp;
	}
}
