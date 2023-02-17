using System.Collections;
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
    private float _elapsedTime = 0;
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
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _nextWave)
            {
                StartCoroutine(SpawnWaveCoroutine(_waveQueue.Dequeue()));
                _nextWave = NextWaveDelay();
            }
        }
    }

    //private void SpawnWave(SpiritWave wave)
    //{
    //    var spawnLocations = new Vector3[wave.Count];
    //    for (int i = 0; i < wave.Count; i++)
    //    {
    //        // Random seed seems to always be the same and is an awkward seed
    //        // Should think about how we're handling randoms
    //        int rand = Random.Range(0, _spawnPoints.Length);
    //        spawnLocations[i] = _spawnPoints[rand].transform.position;
    //        Instantiate(_spirit, spawnLocations[i] + 3 * Utility.RandomUnitVec3().ZeroY(), Quaternion.identity, transform);
    //    }
    //
    //    //	Alert any interested parties that a wave has spawned
    //    _worldEvents?.OnSpiritWaveSpawned(spawnLocations);
    //}

    private IEnumerator SpawnWaveCoroutine(SpiritWave wave)
    {
        var spawnLocations = new Vector3[wave.Count];
        for (int i = 0; i < wave.Count; i++)
        {
            int rand = Random.Range(0, _spawnPoints.Length);
            var pos = _spawnPoints[rand].transform.position + 3 * Utility.RandomUnitVec3().ZeroY();
            spawnLocations[i] = pos;
            Instantiate(_spirit, pos, Quaternion.identity, transform).TryGetComponent(out Spirit spirit);
            spirit?.SetPropsOnSpawn(wave.MoveSpeedMultiplier, wave.PossessionRateMultiplier);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
        //	Alert any interested parties that a wave has spawned
        _worldEvents?.OnSpiritWaveSpawned(spawnLocations);
    }

    private float NextWaveDelay() => _waveQueue.Count > 0 ? _nextWave + _waveQueue.Peek().Delay : 0.0f;
}
