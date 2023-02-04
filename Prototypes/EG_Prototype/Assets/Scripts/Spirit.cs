using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour, IInteractable
{
    private Plant _targetPlant;

    [SerializeField]
    private GameObject _spawnedGO;

    [SerializeField]
    private GameObject _spawningGO;

    private SpiritState _spiritState;

    [SerializeField]
    private float _spawnDelay;

    enum SpiritState
    {
        Spawning,
        Possessing,
        Stunned
    }

    // Start is called before the first frame update
    void Start()
    {
        SpiritManager.instance.RegisterSpirit(this);
        _spiritState = SpiritState.Spawning;
        _spawnedGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_spiritState)
        {
            case SpiritState.Spawning:
                _spawnDelay -= Time.deltaTime;
                if (_spawnDelay <= 0)
                {
                    Spawn();
                }
                break;
            case SpiritState.Stunned:
                break;
            default:
                break;
        }
    }

    void OnDisable()
    {
        SpiritManager.instance.DeregisterSpirit(this);
    }

    void Spawn()
    {
        _spawningGO.SetActive(false);
        //if the target plant is at the spirit's location, possess the target plant
        if (_targetPlant.transform.position == transform.position && _targetPlant.IsPossessable())
        {
            Possess(_targetPlant);
        }
        // else spawn the spirit in the world and stun it
        else
        {
            _spawnedGO.SetActive(true);
            _spiritState = SpiritState.Stunned;
        }
    }

    void Possess(Plant plant)
    {
        plant.OnPossessed(this);
        _spiritState = SpiritState.Possessing;
        gameObject.SetActive(false);
    }

    public void OnDispossess(Vector3 position)
    {
        _spiritState = SpiritState.Stunned;
        _spawnedGO.SetActive(true);
        gameObject.transform.position = position;
        gameObject.SetActive(true);
    }

    public void SetTarget(Plant target)
    {
        _targetPlant = target;
    }

    public bool IsInteractable()
    {
        if (_spiritState == SpiritState.Stunned)
        {
            return true;
        }
        return false;
    }

    public void OnPlayerInteract(PlayerController player)
    {
        switch (_spiritState)
        {
            case SpiritState.Stunned:
                GameManager.instance.ScorePoints(10);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
