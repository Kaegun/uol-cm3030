using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpiritSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject _spirit;

	[SerializeField]
	private Transform[] _spawnPoints;

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	private float _nextWave = 0;
	private Queue<SpiritWave> _waveQueue;

	private const float _minimumSpawnDelay = 0.5f,
		_maximumSpawnDelay = 6.0f;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		_waveQueue = new Queue<SpiritWave>(GameManager.Instance.ActiveLevel.Waves);
		_nextWave = NextWaveDelay();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_waveQueue.Count > 0)
		{
			if (Time.timeSinceLevelLoad > _nextWave)
			{
				StartCoroutine(SpawnWaveCoroutine(_waveQueue.Dequeue()));
				_nextWave = NextWaveDelay();
				Debug.Log($"SpiritSpawner NextWave: {_nextWave}");
			}
		}
	}

	private IEnumerator SpawnWaveCoroutine(SpiritWave wave)
	{
		var spirits = new Spirit[wave.Count];
		for (int i = 0; i < wave.Count; i++)
		{
			int rand = Random.Range(0, _spawnPoints.Length);
			var pos = _spawnPoints[rand].transform.position + 3 * Utility.RandomUnitVec3().ZeroY();
			if (Instantiate(_spirit, pos, Quaternion.identity, transform).TryGetComponent(out spirits[i]))
			{
				spirits[i].SetPropertiesOnSpawn(wave.MoveSpeedMultiplier, wave.PossessionRateMultiplier);
			}

			_worldEvents.OnSpiritSpawned(spirits[i]);
			//	Delay to wait for next spawn in a wave
			yield return new WaitForSeconds(Random.Range(_minimumSpawnDelay, Mathf.Min(wave.SpiritSpawnDelay, _maximumSpawnDelay)));
		}

		//	Alert any interested parties that a wave has spawned
		_worldEvents.OnSpiritWaveSpawned(spirits);
	}

	private float NextWaveDelay() => _waveQueue.Count > 0 ? _nextWave + _waveQueue.Peek().WaveDelay : 0.0f;
}
