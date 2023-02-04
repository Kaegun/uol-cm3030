using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject _spirit;

    [SerializeField]
    private SpiritSpawnPoint[] _spawnPoints;

    [System.Serializable]
    // Class rather than struct to allow mutability when stored in a queue
    class Wave
    {
        // number of spirits spawned in the wave
        public int Count;

        // delay from previous wave to this wave
        public float Delay;
    }

    [SerializeField]
    private Wave[] _waveArray;

    private Queue<Wave> _waveQueue;

    // Start is called before the first frame update
    void Start()
    {
        _waveQueue = new Queue<Wave>(_waveArray);
    }

    // Update is called once per frame
    void Update()
    {
        if (_waveQueue.Count > 0)
        {
            Wave wave = _waveQueue.Peek();
            wave.Delay -= Time.deltaTime;

            if (wave.Delay <= 0)
            {
                SpawnWave(_waveQueue.Dequeue());
                // account for overspill of delay in previous wave
                _waveQueue.Peek().Delay += wave.Delay;
            }
        }        
    }

    void SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.Count; i++)
        {
            int rand = Random.Range(0, _spawnPoints.Length);
            Instantiate(_spirit, _spawnPoints[rand].transform.position, Quaternion.identity);
        }
    }   
}
