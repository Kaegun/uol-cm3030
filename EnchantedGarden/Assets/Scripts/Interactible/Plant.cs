using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Plant : PickUpBase, IPossessable, IInteractable
{
    enum PlantState
    {
        Default,
        BecomingPossessed,
        Carried
    }

    [SerializeField]
    private PlantState _plantState;

    //	TODO: VFX Changes
    [SerializeField]
    private Material _plantMaterial;

    [SerializeField]
    private Material _spiritMaterial;

    [SerializeField]
    private PlantPatch _plantPatch;

    [SerializeField]
    private ScriptableWorldEventHandler _worldEvents;

    //  Amount of time plant has been BeingPossessed. Reset by when dispossessed
    private float _possessionProgress;
    private float _replantingProgress;
    private bool _planted;

    public bool CanBeReplanted()
    {
        return _plantState == PlantState.Default && _plantPatch != null && !_planted;
    }

    //	TODO: Not sure this is correct
    public void Replant(PlantPatch parent)
    {
        _planted = true;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        _plantPatch = parent;
    }

    public bool CanBePossessed => _plantState == PlantState.Default;

    public bool PossessionCompleted => _possessionProgress >= GameManager.Instance.Level.PossessionThreshold;

    public override Transform Transform => transform;

    public GameObject GameObject => gameObject;

    public void OnPossessionStarted(Spirit possessor)
    {
        _plantState = PlantState.BecomingPossessed;
        _worldEvents.OnPlantPossessing(transform.position);
    }

    public void WhileCompletingPossession(Spirit possessor)
    {
        _possessionProgress += _planted ? Time.deltaTime : Time.deltaTime * GameManager.Instance.Level.UnplantedFactor;
        // TODO: Refactor to move smoothly to a specific position on the spirit
        transform.position = Vector3.Lerp(transform.position, possessor.transform.position, Time.deltaTime * _possessionProgress / GameManager.Instance.Level.PossessionThreshold);
    }

    public void OnPossessionCompleted(Spirit possessor)
    {
        _plantState = PlantState.Carried;
        _possessionProgress = GameManager.Instance.Level.PossessionThreshold;
        if (_plantPatch != null)
        {
            _plantPatch.RemovePlant();
            _plantPatch = null;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10));

        _worldEvents.OnPlantPossessed(transform.position);
    }

    public void OnDispossess()
    {
        _plantState = PlantState.Default;
        _possessionProgress = 0;
    }

    //public void StartPossession()
    //{
    //	_plantState = PlantState.BecomingPossessed;
    //	_worldEvents.OnPlantPossessing(transform.position);
    //}

    //	TODO: Property
    //public bool PossessionThresholdReached => _possessionProgress >= GameManager.Instance.Level.PossessionThreshold;

    //public void CompletePossession()
    //{
    //	_plantState = PlantState.Carried;
    //	_possessionProgress = GameManager.Instance.Level.PossessionThreshold;
    //	if (_plantPatch != null)
    //	{
    //		_plantPatch.RemovePlant();
    //		_plantPatch = null;
    //	}
    //	transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10));
    //
    //	_worldEvents.OnPlantPossessed(transform.position);
    //}

    //public void Dispossess()
    //{
    //	_plantState = PlantState.Default;
    //	_possessionProgress = 0;
    //}

    public override bool CanBePickedUp => _plantState == PlantState.Default;

    //	TODO: Convert to Trigger + Layer  
    public override bool CanBeDropped => base.CanBeDropped && Physics.OverlapSphere(transform.position, 2.0f).
                Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant).
                ToList().Count > 0;


    //	This could be a Scriptable Object
    //public Transform PickupAdjustment => throw new System.NotImplementedException();

    public override void OnPickUp(Transform pickupTransform)
    {
        _plantState = PlantState.Carried;
        if (_plantPatch != null)
        {
            _plantPatch.RemovePlant();
            _plantPatch = null;
            _planted = false;
        }

        base.OnPickUp(pickupTransform);
    }

    public override void OnDrop(bool despawn = false)
    {
        base.OnDrop();
        //	TODO: All of this can be done with Trigger Collider and Layers
        var plantPatch = Physics.OverlapSphere(transform.position, 2.0f).
            Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant).
            Select(c => c.GetComponent<PlantPatch>()).
            OrderBy(c => Vector3.Distance(c.transform.position, transform.position)).
            FirstOrDefault();

        if (plantPatch != null)
        {
            _plantPatch = plantPatch;
            transform.position = _plantPatch.transform.position;
        }

        _plantState = PlantState.Default;
        transform.rotation = Quaternion.identity.RandomizeY();
    }

    //  Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(_worldEvents);
        _possessionProgress = 0;
    }

    //  Update is called once per frame
    private void Update()
    {
        //if (_plantState == PlantState.BecomingPossessed)
        //{
        //    _possessionProgress += _planted ? Time.deltaTime : Time.deltaTime * GameManager.Instance.Level.UnplantedFactor;
        //}

        //	TODO: Plant possession VFX needs to change
        //  Alter plant material based on progress towards possession
        //_mesh.material.Lerp(_plantMaterial, _spiritMaterial, _possessionProgress / _possessionThreshold);
    }

    public bool CanInteractWith(IInteractor interactor)
    {
        switch (interactor)
        {
            case Shovel _:
                return CanBeReplanted() && interactor.CanInteractWith(this);
            default:
                return false;
        }
    }

    public void OnInteractWith(IInteractor interactor)
    {
        switch (interactor)
        {
            case Shovel _:
                _replantingProgress += Time.deltaTime;
                if (_replantingProgress >= GameManager.Instance.Level.ReplantingThreshold)
                {
                    Debug.Log("Replanted plant!");
                    _planted = true;
                }
                break;
            default:
                break;
        }

    }
}
