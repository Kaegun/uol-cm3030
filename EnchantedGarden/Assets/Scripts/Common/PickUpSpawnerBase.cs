﻿using UnityEngine;
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

	[Header("Pickup Indicator")]
	[SerializeField]
	private Vector3 _pickUpIndicatorAdjustment;

	[Header("Animations")]
	[SerializeField]
	private bool _playPickUpAnimation;

	public Vector3 IndicatorPosition => transform.position + _pickUpIndicatorAdjustment;

	public bool PlayPickUpAnimation => _playPickUpAnimation;

	public IPickUp SpawnPickUp()
	{
		var pickUp = Instantiate(_spawnedPrefab);
		Assert.IsNotNull(pickUp, Utility.AssertNotNullMessage(nameof(pickUp)));
		return pickUp;
	}
}
