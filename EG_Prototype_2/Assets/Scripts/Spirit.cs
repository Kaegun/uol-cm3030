using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    enum SpiritState
    {
        Searching,
        StartingPossession,
        Possessing
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

                // search for nearby possessable plants and try to possess one
                var plants = Physics.OverlapSphere(transform.position, 2f).
                    Where(c => c.GetComponent<Plant>() != null && c.GetComponent<Plant>().IsPossessable()).
                    Select(c => c.GetComponent<Plant>()).
                    ToList();
                if (plants.Count > 0)
                {
                    _possessedPlant = plants[0];
                    _possessedPlant.StartPossession();
                    transform.position = _possessedPlant.transform.position;
                    _spiritState = SpiritState.StartingPossession;
                    _spiritBody.SetActive(false);
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
            default:
                break;
        }
    }

    public bool IsBanishable()
    {
        return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
    }

    public void Banish()
    {
        if (IsBanishable())
        {
            _possessedPlant.Dispossess();
            Destroy(gameObject);
        }
    }
}
