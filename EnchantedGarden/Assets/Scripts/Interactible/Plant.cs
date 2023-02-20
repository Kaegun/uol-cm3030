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
	private GameObject _plantModel;

	[Header("Normal Plant Model Transform")]
	[SerializeField]
	private Vector3 _normalPosition;
	[SerializeField]
	private Vector3 _normalRotation;
	[SerializeField]
	private Vector3 _normalScale;

	[Header("Dropped Plant Model Transform")]
	[SerializeField]
	private Vector3 _droppedPosition;
	[SerializeField]
	private Vector3 _droppedRotation;
	[SerializeField]
	private Vector3 _droppedScale;

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	//  Amount of time plant has been BeingPossessed. Reset by when dispossessed
	private float _possessionProgress;
	private float _replantingProgress;
	private bool _planted;
	private Vector3 _startPossessionPos;

	public bool CanBeReplanted => _plantState == PlantState.Default && _plantPatch != null && !_planted;

	public void Replant(PlantPatch parent)
	{
		_planted = true;
		_plantPatch = parent;
		_replantingProgress = 0;
		SetModelNormal();
	}

	public bool CanBePossessed => _plantState == PlantState.Default;

	public bool PossessionCompleted => _possessionProgress >= GameManager.Instance.ActiveLevel.PossessionThreshold;

	public override Transform Transform => transform;

	public GameObject GameObject => gameObject;

	public void OnPossessionStarted(Spirit possessor)
	{
		SetModelNormal();
		_plantState = PlantState.BecomingPossessed;
		_startPossessionPos = transform.position;
		_worldEvents.OnPlantPossessing(transform.position);
	}

	public void WhileCompletingPossession(Spirit possessor)
	{
		_possessionProgress += possessor.PossessionRateMultiplier * (_planted ? Time.deltaTime : Time.deltaTime * GameManager.Instance.ActiveLevel.UnplantedFactor);
		transform.position = Vector3.Lerp(_startPossessionPos, possessor.transform.position, _possessionProgress / GameManager.Instance.ActiveLevel.PossessionThreshold);
	}

	public void OnPossessionCompleted(Spirit possessor)
	{
		_plantState = PlantState.Carried;
		_possessionProgress = GameManager.Instance.ActiveLevel.PossessionThreshold;
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
		transform.position = transform.position.ZeroY();
		if (_plantPatch != null)
		{
			SetModelNormal();
		}
		else
		{
			SetModelDropped();
		}
	}

	public override bool CanBePickedUp => _plantState == PlantState.Default && !_planted;

	//	TODO: Convert to Trigger + Layer  
	public override bool CanBeDropped => base.CanBeDropped && Physics.OverlapSphere(transform.position, 2.0f).
				Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant).
				ToList().Count > 0;


	public override void OnPickUp(Transform pickupTransform)
	{
		SetModelNormal();
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

		SetModelDropped();
		_plantState = PlantState.Default;
	}

	//  Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		_possessionProgress = 0;
	}

	//  Update is called once per frame
	protected override void Update()
	{
		//if (_plantState == PlantState.BecomingPossessed)
		//{
		//    _possessionProgress += _planted ? Time.deltaTime : Time.deltaTime * GameManager.Instance.Level.UnplantedFactor;
		//}

		//	TODO: Plant possession VFX needs to change
		//  Alter plant material based on progress towards possession
		//_mesh.material.Lerp(_plantMaterial, _spiritMaterial, _possessionProgress / _possessionThreshold);
	}

	private void SetModelNormal()
	{
		_plantModel.transform.localPosition = _normalPosition;
		_plantModel.transform.localRotation = Quaternion.Euler(_normalRotation);
		_plantModel.transform.localScale = _normalScale;
	}

	private void SetModelDropped()
	{
		_plantModel.transform.localPosition = _droppedPosition;
		_plantModel.transform.localRotation = Quaternion.Euler(_droppedRotation);
		_plantModel.transform.localScale = _droppedScale;
	}

	public bool CanInteractWith(IInteractor interactor)
	{
		switch (interactor)
		{
			case Shovel _:
				return CanBeReplanted && interactor.CanInteractWith(this);
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
				if (_replantingProgress >= GameManager.Instance.ActiveLevel.ReplantingThreshold)
				{
					Debug.Log("Replanted plant!");
					Replant(_plantPatch);
				}
				break;
			default:
				break;
		}
	}

	public bool DestroyOnInteract(IInteractor interactor)
	{
		return false;
	}
}
