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

	public bool ContainsCompost()
	{
		return _containsCompost;
	}

	public void AddCompost()
	{
		_containsCompost = true;
		_mesh.material = _compostMaterial;
	}

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
			_plant = Instantiate(_plantPrefab, transform.position, Quaternion.identity);
		}
	}
}
