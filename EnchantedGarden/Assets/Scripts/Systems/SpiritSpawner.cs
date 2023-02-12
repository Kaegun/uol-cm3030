using System.Collections.Generic;
using UnityEngine;

public class SpiritSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject _spirit;

	[SerializeField]
	private ScriptableLevelDefinition _level;

	[SerializeField]
	private Transform[] _spawnPoints;

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	private float _nextWave = 0;
	private Queue<SpiritWave> _waveQueue;

	// Start is called before the first frame update
	private void Start()
	{
		_waveQueue = new Queue<SpiritWave>(_level.Waves);
		_nextWave = NextWaveDelay();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_waveQueue.Count > 0)
		{
			var wave = _waveQueue.Peek();
			_nextWave += Time.deltaTime;
			if (_nextWave > wave.Delay)
			{
				SpawnWave(_waveQueue.Dequeue());
				//	account for overspill of delay in previous wave (TODO: see Next WaveDelay)
				_nextWave = NextWaveDelay();
			}
		}
	}

	private void SpawnWave(SpiritWave wave)
	{
		var spawnLocations = new Vector3[wave.Count];
		for (int i = 0; i < wave.Count; i++)
		{
			int rand = Random.Range(0, _spawnPoints.Length);
			spawnLocations[i] = _spawnPoints[rand].transform.position;
			Instantiate(_spirit, spawnLocations[i] + 3 * Utility.RandomUnitVec3().ZeroY(), Quaternion.identity, transform);
		}

		//	Alert any interested parties that a wave has spawned
		_worldEvents?.OnSpiritWaveSpawned(spawnLocations);
	}

	private float NextWaveDelay() => _waveQueue.Count > 0 ? _nextWave + _waveQueue.Peek().Delay : 0.0f;
}
