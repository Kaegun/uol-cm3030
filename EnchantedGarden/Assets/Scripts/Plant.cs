using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
	private float _unplantedFactor = 10.0f;

	//  Threshold for amount of time plant must be BeingPossessed state before becoming Possessed
	[SerializeField]
	private float _possessionThreshold;

	//  Amount of time plant has been BeingPossessed. Reset by when dispossessed
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

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

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
		_worldEvents?.OnPlantPossessing(transform.position);
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

		_worldEvents?.OnPlantPossessed(transform.position);
	}

	public void Dispossess()
	{
		_plantState = PlantState.Default;
		_possessionProgress = 0;
	}

	public bool CanBePickedUp => _plantState == PlantState.Default;

	public bool CanBeDropped
	{
		get
		{
			var plantPatches = Physics.OverlapSphere(transform.position, 2.0f).
				Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant()).
				ToList();
			return plantPatches.Count > 0;
		}
	}

	public void OnPickUp() { }

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

	//  Start is called before the first frame update
	private void Start()
	{
		_possessionProgress = 0;
	}

	//  Update is called once per frame
	private void Update()
	{
		if (_plantState == PlantState.BecomingPossessed)
		{
			_possessionProgress += _planted ? Time.deltaTime : Time.deltaTime * _unplantedFactor;
		}

		//  Alter plant material based on progress towards possession
		_mesh.material.Lerp(_plantMaterial, _spiritMaterial, _possessionProgress / _possessionThreshold);
	}
}
