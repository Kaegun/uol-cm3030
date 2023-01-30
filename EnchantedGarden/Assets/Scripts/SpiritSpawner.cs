using System.Collections.Generic;
using UnityEngine;

public class SpiritSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject _spirit;

	//[SerializeField]
	//private SpiritSpawnPoint[] _spawnPoints;

	private Queue<SpiritWave> _waveQueue;

	[SerializeField]
	private ScriptableLevelDefinition _level;

	[SerializeField]
	private Transform[] _spawnPoints;

	private float _nextWave = 0;

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
				// account for overspill of delay in previous wave
				_nextWave = NextWaveDelay();
			}
		}
	}

	private void SpawnWave(SpiritWave wave)
	{
		for (int i = 0; i < wave.Count; i++)
		{
			int rand = Random.Range(0, _spawnPoints.Length);
			Instantiate(_spirit, _spawnPoints[rand].transform.position, Quaternion.identity);
		}
	}

	private float NextWaveDelay() => _waveQueue.Count > 0 ? _waveQueue.Peek().Delay : 0.0f;
}
