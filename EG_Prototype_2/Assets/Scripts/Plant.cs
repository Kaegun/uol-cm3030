using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    enum PlantState
    {
        Planted,
        BecomingPossessed,
        Carried
    }

    [SerializeField]
    private PlantState _plantState;

    [SerializeField]
    // Threshold for amount of time plant must be BeingPossessed state before becoming Possessed
    private float _possessionThreshold;

    // Amount of time plant has been BeingPossessed. Reset by when dispossessed
    private float _possessionProgress = 0;

    [SerializeField]
    private Material _plantMaterial;

    [SerializeField]
    private Material _spiritMaterial;

    [SerializeField]
    private MeshRenderer _mesh;

    [SerializeField]
    private PlantPatch _plantPatch;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (_plantState)
        {
            case PlantState.Planted:
                break;
            case PlantState.BecomingPossessed:
                _possessionProgress += Time.deltaTime;                
                break;
            case PlantState.Carried:
                break;
            default:
                break;
        }

        // Alter plant material based on progress towards possession
        _mesh.material.Lerp(_plantMaterial, _spiritMaterial, _possessionProgress / _possessionThreshold);
    }

    public bool IsPossessable()
    {
        return _plantState == PlantState.Planted;
    }

    public bool IsCarriable()
    {
        return _plantState == PlantState.Planted;
    }

    public void StartPossession()
    {
        _plantState = PlantState.BecomingPossessed;
    }

    public bool PossessionThresholdReached()
    {
        return _possessionProgress >= _possessionThreshold;
    }

    public void CompletePossession()
    {
        _plantState = PlantState.Carried;
        _possessionProgress = _possessionThreshold;
        _plantPatch.RemovePlant();
        _plantPatch = null;        
    }

    public void Dispossess()
    {
        _plantState = PlantState.Planted;
        _possessionProgress = 0;        
    }
}
