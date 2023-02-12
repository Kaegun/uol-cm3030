using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class StaticSpawnedObject
{
	[SerializeField]
	private GameObject _prefab;
	public GameObject Prefab => _prefab;

	[SerializeField]
	private Transform _position;
	public Transform Position => _position;

	[SerializeField]
	private float _localScale = 1.0f;
	public float LocalScale => _localScale;

	[SerializeField]
	private float _spawnRadius = 1.0f;
	public float SpawnRadius => _spawnRadius;

	[SerializeField]
	private int _numberToSpawn = 1;
	public int NumberToSpawn => _numberToSpawn;

	public static void Valid(StaticSpawnedObject spawnedObject)
	{
		Assert.IsNotNull(spawnedObject);
		Assert.IsNotNull(spawnedObject.Prefab);
		Assert.IsNotNull(spawnedObject.Position);
	}

	public List<GameObject> Spawn()
	{
		List<GameObject> list = new List<GameObject>();

		if (_numberToSpawn > 1)
		{
			//	TODO: Fix this
			//	Spawn a couple of objects around the spawn point
			float stepSize = _spawnRadius / _numberToSpawn;
			Vector3 offset = new Vector3(stepSize / 2, 0, stepSize / 2);

			var rowsCols = Mathf.CeilToInt(_numberToSpawn / 2.0f);
			Debug.Log($"Spawning ({rowsCols}): {_prefab.name}");
			for (int i = 0; i < rowsCols; i++)
			{
				//	TODO: Position the objects correctly
				for (int j = 0; j < rowsCols; j++)
				{
					if ((i + 1) * (j + 1) + j > _numberToSpawn)
						break;

					var spawned = Instantiate();
					//	TODO: Calculate offsets - Start at the front of the table
					spawned.transform.position += offset - new Vector3(i * stepSize, 0, j * stepSize);
					list.Add(spawned);
				}
			}
		}
		else
		{
			list.Add(Instantiate());
		}

		return list;
	}

	private GameObject Instantiate()
	{
		var spawned = GameObject.Instantiate(_prefab, _position);
		spawned.transform.localScale *= _localScale;

		if (spawned.TryGetComponent<IPickUp>(out var pickup))
			pickup.CanBePickedUp = pickup.Despawns = false;

		return spawned;
	}
}
