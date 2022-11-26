using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour, IInteractable
{
    private PlantState _plantState;

    [SerializeField]
    private Material _plantMaterial;

    [SerializeField]
    private Material _spiritMaterial;

    private Spirit _possessingSpirit;
    private Vector3 _direction;

    enum PlantState
    {
        Planted,
        Possessed
    }

    // Start is called before the first frame update
    void Start()
    {
        SpiritManager.instance.RegisterPlant(this);
        _plantState = PlantState.Planted;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_plantState)
        {
            case PlantState.Planted:
                break;
            case PlantState.Possessed:
                transform.position += 1.5f * Time.deltaTime * _direction;
                break;
            default:
                break;
        }
    }

    void OnDisable()
    {
        SpiritManager.instance.DeregisterPlant(this);
    }

    public bool IsPossessable()
    {
        if (_plantState == PlantState.Planted)
        {
            return true;
        }
        return false;
    }

    public void OnPossessed(Spirit spirit)
    {
        _plantState = PlantState.Possessed;
        _possessingSpirit = spirit;
        _direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        gameObject.GetComponentInChildren<MeshRenderer>().material = _spiritMaterial;
    }

    void Dispossess()
    {
        _plantState = PlantState.Planted;
        gameObject.GetComponentInChildren<MeshRenderer>().material = _plantMaterial;
        _possessingSpirit.OnDispossess(transform.position + new Vector3(1.5f, 0, 0));
    }

    public bool IsInteractable()
    {
        if (_plantState == PlantState.Planted || _plantState == PlantState.Possessed)
        {
            return true;
        }
        return false;
    }

    public void OnPlayerInteract(PlayerController player)
    {
        switch (_plantState)
        {
            case PlantState.Planted:
                break;
            case PlantState.Possessed:
                Dispossess();
                break;
            default:
                break;
        }
    }
}
