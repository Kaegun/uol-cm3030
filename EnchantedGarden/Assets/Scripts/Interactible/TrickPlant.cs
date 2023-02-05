using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrickPlant : MonoBehaviour, IPickUp, IInteractable
{
	enum PlantState
	{
		Inactive,
		Planted,
		TrappingSpirit,
	}

	[SerializeField]
	private PlantState _plantState;

	private PlantPatch _plantPatch;

	[SerializeField]
	private float _growthDuration;
	private float _growthProgress = 0;
	private bool _fullyGrown = false;

	[SerializeField]
	private float _trapDuration;
	private float _trapProgress = 0;
	private Spirit _trappedSpirit;

	[SerializeField]
	private MeshRenderer _mesh;

	[SerializeField]
	private Material _normalMaterial;
	[SerializeField]
	private Material _trappedMaterial;

	public bool CanTrapSpirit => _fullyGrown && _plantState == PlantState.Planted;

	public void TrapSpirit(Spirit spirit)
	{
		_trappedSpirit = spirit;
		_plantState = PlantState.TrappingSpirit;
	}

	public PlantPatch PlantPatch()
	{
		return _plantPatch;
	}

	public bool CanBeDropped => true;

	public bool CanBePickedUp => _plantState == PlantState.Inactive || _plantState == PlantState.Planted;

	public void OnDrop()
	{
		//	TODO: Convert to use Trigger and Layer
		var plantPatches = Physics.OverlapSphere(transform.position, 2.0f).
			Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsPlant).
			Select(c => c.GetComponent<PlantPatch>()).
			OrderBy(c => Vector3.Distance(c.transform.position, transform.position)).
			ToList();

		if (plantPatches.Count > 0)
		{
			_plantPatch = plantPatches[0];
			transform.position = _plantPatch.transform.position;
			_plantState = PlantState.Planted;
		}
		else
		{
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
			_plantState = PlantState.Inactive;
		}
	}

	public void OnPickUp()
	{
		_plantState = PlantState.Inactive;
		if (_plantPatch != null)
		{
			_plantPatch.RemovePlant();
			_plantPatch = null;
		};
	}

	public GameObject PickUpObject()
	{
		return gameObject;
	}

	public bool IsInteractable => _plantState == PlantState.TrappingSpirit;

	public void OnPlayerInteract(PlayerInteractionController player)
	{
		_trappedSpirit.Banish();
		_plantState = PlantState.Planted;
		_trapProgress = 0;
	}

	//	Update is called once per frame
	private void Update()
	{
		switch (_plantState)
		{
			case PlantState.Inactive:
				break;
			case PlantState.Planted:
				if (!_fullyGrown)
				{
					_growthProgress += Time.deltaTime;
					if (_growthProgress >= _growthDuration)
					{
						_growthProgress = _growthDuration;
						_fullyGrown = true;
					}
				}
				break;
			case PlantState.TrappingSpirit:
				_trapProgress += Time.deltaTime;
				if (_trapProgress >= _trapDuration)
				{
					_trappedSpirit.Banish();
					_plantPatch.RemovePlant();
					Destroy(gameObject);
				}
				break;
			default:
				break;
		}

		// Change scale based on growth progress
		gameObject.transform.localScale = Vector3.one * System.Math.Min(0.5f + _growthProgress / _growthDuration, 1);

		// Lerp material based on spirit trapped
		_mesh.material.Lerp(_normalMaterial, _trappedMaterial, _trapProgress / _trapDuration);
	}
}
