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
	private float _elapsedTime = 0;
	private Queue<SpiritWave> _waveQueue;

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
			_elapsedTime += Time.deltaTime;
			if (_elapsedTime > _nextWave)
			{
				StartCoroutine(SpawnWaveCoroutine(_waveQueue.Dequeue()));
				_nextWave = NextWaveDelay();
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
				spirits[i].SetPropsOnSpawn(wave.MoveSpeedMultiplier, wave.PossessionRateMultiplier);
			_worldEvents.OnSpiritSpawned(spirits[i]);
			//	Delay to wait for next spawn in a wave
			yield return new WaitForSeconds(wave.Delay == 0.0f ? Random.Range(1.0f, 3.0f) : Random.Range(Mathf.Min(0.5f, wave.Delay), wave.Delay));
		}

		//	Alert any interested parties that a wave has spawned
		_worldEvents.OnSpiritWaveSpawned(spirits);
	}

	private float NextWaveDelay() => _waveQueue.Count > 0 ? _nextWave + _waveQueue.Peek().Delay : 0.0f;
}
