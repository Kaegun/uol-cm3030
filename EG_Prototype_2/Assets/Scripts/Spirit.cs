using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spirit : MonoBehaviour, IInteractable
{
    enum SpiritState
    {
        Searching,
        StartingPossession,
        Possessing,
        Trapped,
        Repelled
    }

    [SerializeField]
    private SpiritState _spiritState;

    [SerializeField]
    private float _moveSpeed;
    private Vector3 _moveDirection;
    private float _moveTime = 0;

    private Plant _possessedPlant;

    [SerializeField]
    private GameObject _spiritBody;

    [SerializeField]
    private float _repelDuration;
    private float _repelProgress;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (_spiritState)
        {
            case SpiritState.Searching:
                // random movement for testing purposes
                _moveTime += Time.deltaTime;
                if (_moveTime >= Random.Range(2.5f, 4f))
                {
                    _moveDirection = (transform.position.normalized * -1 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized).normalized;
                    _moveDirection.y = 0;
                    _moveTime = 0;
                }
                transform.position += _moveSpeed * Time.deltaTime * _moveDirection;

                // check for nearby possessable plants and start to possess one
                var plants = Physics.OverlapSphere(transform.position, 2f).
                    Where(c => (c.GetComponent<Plant>() != null && c.GetComponent<Plant>().CanBePossessed()) || (c.GetComponent<TrickPlant>() != null && c.GetComponent<TrickPlant>().CanTrapSpirit())).
                    ToList();
                if (plants.Count > 0)
                {
                    // handle normal plant
                    if (plants[0].GetComponent<Plant>() != null)
                    {
                        var plant = plants[0].GetComponent<Plant>();
                        if (plant.PlantPatch() != null && plant.PlantPatch().ContainsCompost())
                        {
                            Repel();
                            plant.PlantPatch().RemoveCompost();
                        }
                        else
                        {
                            _possessedPlant = plant;
                            _possessedPlant.StartPossession();
                            transform.position = _possessedPlant.transform.position;
                            _spiritState = SpiritState.StartingPossession;
                            _spiritBody.SetActive(false);
                        }
                    }
                    // handle trick plant
                    else if (plants[0].GetComponent<TrickPlant>() != null)
                    {
                        var trickPlant = plants[0].GetComponent<TrickPlant>();
                        if (trickPlant.PlantPatch() != null && trickPlant.PlantPatch().ContainsCompost())
                        {
                            Repel();
                            trickPlant.PlantPatch().RemoveCompost();
                        }
                        else
                        {
                            trickPlant.TrapSpirit(this);
                            _spiritBody.SetActive(false);
                            _spiritState = SpiritState.Trapped;
                            transform.position = trickPlant.transform.position;
                        }
                    }
                }
                break;
            case SpiritState.StartingPossession:
                if (_possessedPlant.PossessionThresholdReached())
                {
                    _possessedPlant.CompletePossession();
                    _spiritState = SpiritState.Possessing;
                    _moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                }
                break;
            case SpiritState.Possessing:
                transform.position += _moveSpeed * Time.deltaTime * _moveDirection;
                _possessedPlant.transform.position = transform.position;
                break;
            case SpiritState.Repelled:
                _repelProgress += Time.deltaTime;
                if (_repelProgress > _repelDuration)
                {
                    _spiritState = SpiritState.Searching;
                }
                transform.position += _moveSpeed * 2.5f * Time.deltaTime * _moveDirection;
                break;
            default:
                break;
        }
    }

    public bool CanBeBanished()
    {
        return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
    }

    public void Banish()
    {
        if (_possessedPlant != null)
        {
            _possessedPlant.Dispossess();
        }
        Destroy(gameObject);
    }

    public bool CanBeRepelled()
    {
        return _spiritState == SpiritState.Searching || _spiritState == SpiritState.Repelled;
    }

    public void Repel()
    {
        _spiritState = SpiritState.Repelled;
        _repelProgress = 0;
        _moveDirection = new Vector3(0, 0, 1).normalized;
    }

    public bool CanBeInteractedWith()
    {
        return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
    }

    public void OnPlayerInteract(PlayerController player)
    {
        Banish();
    }
}
