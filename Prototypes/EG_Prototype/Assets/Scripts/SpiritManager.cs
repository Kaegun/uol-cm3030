using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpiritManager : MonoBehaviour
{
    public static SpiritManager instance;

    List<Plant> plants;
    List<Spirit> spirits;

    [SerializeField]
    private float _spawnInterval;

    private float _elapsedTime;

    [SerializeField]
    private GameObject _spiritGO;

    void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        plants = new List<Plant>();
        spirits = new List<Spirit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _spawnInterval && plants.Count > 0)
        {
            // find valid plant targets
            var spiritPositions = spirits.Select((s) => s.transform.position);
            var validPlants = plants.Where(p => !spiritPositions.Contains(p.transform.position) && p.IsPossessable()).ToList();

            // if there are valid plants spawn spirit under one of them
            if (validPlants.Count > 0)
            {
                int rand = Random.Range(0, validPlants.Count);
                var spirit = Instantiate(_spiritGO, validPlants[rand].transform.position, Quaternion.identity);
                spirit.GetComponent<Spirit>().SetTarget(validPlants[rand]);

                _elapsedTime -= _spawnInterval;
            }
        }

        // if all plants gone, end game
        if (plants.Count == 0)
        {
            GameManager.instance.EndGame();
        }
    }

    public void RegisterPlant(Plant plant)
    {
        if (plant != null && !plants.Contains(plant))
        {
            plants.Add(plant);
        }
    }

    public void DeregisterPlant(Plant plant)
    {
        plants.Remove(plant);
    }

    public void RegisterSpirit(Spirit spirit)
    {
        if (spirit != null && !spirits.Contains(spirit))
        {
            spirits.Add(spirit);
        }
    }

    public void DeregisterSpirit(Spirit spirit)
    {
        spirits.Remove(spirit);
    }
}
