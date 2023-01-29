using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant : MonoBehaviour, IPickUp
{
    enum PlantState
    {
        Default,
        BecomingPossessed,
        Carried
    }

    [SerializeField]
    private PlantState _plantState;

    [SerializeField]
    // Threshold for amount of time plant must be BeingPossessed state before becoming Possessed
    private float _possessionThreshold;
    // Amount of time plant has been BeingPossessed. Reset by when dispossessed
    private float _possessionProgress;

    [SerializeField]
    private Material _plantMaterial;
    [SerializeField]
    private Material _spiritMaterial;
    [SerializeField]
    private MeshRenderer _mesh;

    [SerializeField]
    private bool _planted;
    [SerializeField]
    private PlantPatch _plantPatch;

    // Start is called before the first frame update
    void Start()
    {
        _possessionProgress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_plantState == PlantState.BecomingPossessed)
        {
            if (_planted)
            {
                _possessionProgress += Time.deltaTime;
            }
            else
            {
                _possessionProgress += Time.deltaTime * 10;
            }
        }

        // Alter plant material based on progress towards possession
        _mesh.material.Lerp(_plantMaterial, _spiritMaterial, _possessionProgress / _possessionThreshold);
    }

    public bool CanBeReplanted()
    {
        return _plantState == PlantState.Default && _plantPatch != null && !_planted;
    }

    public void Replant()
    {
        _planted = true;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public bool CanBePossessed()
    {
        return _plantState == PlantState.Default;
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

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10));
    }

    public void Dispossess()
    {
        _plantState = PlantState.Default;
        _possessionProgress = 0;
    }

    public bool CanBePickedUp()
    {
        return _plantState == PlantState.Default;
    }

    public bool CanBeDropped()
    {
        var plantPatches = Physics.OverlapSphere(transform.position, 2.0f).
            Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant()).
            ToList();
        return plantPatches.Count > 0;
    }

    public void OnPickUp()
    {

    }

    public void OnDrop()
    {
        var plantPatches = Physics.OverlapSphere(transform.position, 2.0f).
            Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant()).
            Select(c => c.GetComponent<PlantPatch>()).
            OrderBy(c => Vector3.Distance(c.transform.position, transform.position)).
            ToList();

        if (plantPatches.Count > 0)
        {
            _plantPatch = plantPatches[0];
            transform.position = _plantPatch.transform.position;
        }

        _plantState = PlantState.Default;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10));
    }

    public GameObject PickUpObject()
    {
        _plantState = PlantState.Carried;
        if (_plantPatch != null)
        {
            _plantPatch.RemovePlant();
            _plantPatch = null;
            _planted = false;            
        }
        return gameObject;
    }
}
