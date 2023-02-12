using UnityEngine;

public class TrickPlant : PickUpBase
{
	enum PlantState
	{
		Inactive,
		Planted,
		TrappingSpirit,
	}

	[SerializeField]
	private float _growthDuration = 3.0f;

	[SerializeField]
	private float _growthTarget = 2.0f;

	[SerializeField]
	private float _trapDuration = 5.0f;

	//	TODO: VFX
	[SerializeField]
	private Material _normalMaterial;

	[SerializeField]
	private Material _trappedMaterial;

	private PlantState _plantState;
	private float _growthProgress = 0;
	private float _trapProgress = 0;
	private Spirit _trappedSpirit;

	public bool CanTrapSpirit => FullyGrown && _plantState == PlantState.Planted;

	private bool FullyGrown => _growthProgress >= _growthDuration;

	public void TrapSpirit(Spirit spirit)
	{
		_trappedSpirit = spirit;
		_plantState = PlantState.TrappingSpirit;
	}

	public new bool CanBePickedUp => _plantState == PlantState.Inactive || _plantState == PlantState.Planted;

	//	TODO: Change to use trigger collider
	public override void OnDrop(bool despawn = false)
	{
		//	Can be dropped anywhere
		_plantState = PlantState.Planted;
		_canBePickedUp = false;

		base.OnDrop();
	}

	//	TODO: Consider passing in the attach point to this method
	public override void OnPickUp(Transform pickupTransform)
	{
		_plantState = PlantState.Inactive;
		base.OnPickUp(pickupTransform);
	}

	//	Update is called once per frame
	private void Update()
	{
		switch (_plantState)
		{
			case PlantState.Inactive:
				break;
			case PlantState.Planted:
				if (!FullyGrown)
				{
					transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(_growthTarget, _growthTarget, _growthTarget), (_growthProgress / _growthDuration));
					_growthProgress += Time.deltaTime;
				}
				break;
			case PlantState.TrappingSpirit:
				_trapProgress += Time.deltaTime;
				if (_trapProgress >= _trapDuration)
				{
					_trappedSpirit.Banish();
					Destroy(gameObject);
				}
				break;
			default:
				break;
		}

		//	TODO: Replace with VFX 
		//	Lerp material based on spirit trapped
		//_mesh.material.Lerp(_normalMaterial, _trappedMaterial, _trapProgress / _trapDuration);
	}
}
