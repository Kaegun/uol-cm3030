using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PlantPatch : MonoBehaviour
{
	[SerializeField]
	private bool _containsPlant;

	[SerializeField]
	private Plant _plantPrefab;

	[SerializeField]
	private Material _dirtMaterial;

	[SerializeField]
	private Material _compostMaterial;

	private bool _containsCompost = false;
	[Obsolete("Removing Compost")]
	private MeshRenderer _mesh;
	private Plant _plant;

	public bool ContainsPlant => _containsPlant;

	public void AddPlant(Plant plant)
	{
		_containsPlant = true;
		_plant = plant;
	}

	public void RemovePlant()
	{
		_containsPlant = false;
		_plant = null;
	}

	[Obsolete("Removing Compost")]
	public bool ContainsCompost()
	{
		return _containsCompost;
	}

	[Obsolete("Removing Compost")]
	public void AddCompost()
	{
		_containsCompost = true;
		_mesh.material = _compostMaterial;
	}

	[Obsolete("Removing Compost")]
	public void RemoveCompost()
	{
		_containsCompost = false;
		_mesh.material = _dirtMaterial;
	}

	private void Start()
	{
		_mesh = GetComponentInChildren<MeshRenderer>();
		Assert.IsNotNull(_mesh);

		//	Instantiate a plant at the start of the level if flag is set
		if (_containsPlant)
		{
			//	TODO: Instantiate on the "planted" spot, set plant state to planted
			_plant = Instantiate(_plantPrefab, transform.position, Quaternion.identity.RandomizeY());
			_plant.Replant(this);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		//	TODO: This doesn not work as expected
		if (TryGetComponent<DrawGizmoText>(out var gizmo))
		{
			if (_containsPlant)
			{
				gizmo.Text += " - Spawns Plant";
			}
		}
	}
#endif
}
