using UnityEngine;
using UnityEngine.Assertions;

public class FoxBehaviour : MonoBehaviour
{
	/*
	 * This behaviour must listen for world events, and then instruct the Fox to react to them
	 * Test 1 - When a spirit Spawns
	 * 
	 * 
	 */
	private enum FoxState
	{
		Idle,
		Follow,
		Alert,
	}

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[SerializeField]
	private Transform _player;

	[SerializeField]
	private float _turnSpeed = 20.0f;

	[SerializeField]
	private float _maxSpeed = 6.0f;

	[SerializeField]
	private float _acceleration = 6.0f;

	private Animator _animator;
	private FoxState _state;
	private float _currentSpeed = 0.0f;

	private readonly float _idleFollowDistance = 2.0f;

	// Start is called before the first frame update
	private void Start()
	{
		_worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
		_worldEvents.PlantPossessing += PlantPossessing;
		_worldEvents.PlantPossessed += PlantPossessed;

		_animator = GetComponentInChildren<Animator>();
		Assert.IsNotNull(_animator);
	}

	// Update is called once per frame
	private void Update()
	{
		switch (_state)
		{
			case FoxState.Idle:
				//	Align fox's view to player's
				if (!Mathf.Approximately(Vector3.Dot(transform.position, _player.forward), 0))
				{
					transform.rotation = transform.rotation.RotateTowards(transform.position, _player.forward, _turnSpeed * Time.deltaTime);

					//	TODO: Set turning anim.
				}

				_state = (_player.position - transform.position).magnitude > _idleFollowDistance ? FoxState.Follow : FoxState.Idle;
				break;

			case FoxState.Follow:
				//	Move the fox towards the player, whenever no other state is active
				_currentSpeed = Mathf.Clamp(_currentSpeed + _acceleration * Time.deltaTime, 0.0f, _maxSpeed);

				transform.rotation = transform.rotation.RotateTowards(transform.position, _player.position, _turnSpeed * Time.deltaTime);
				transform.position = Vector3.MoveTowards(transform.position, _player.position, _currentSpeed * Time.deltaTime);
				break;
			case FoxState.Alert:
				break;
		}

		_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _currentSpeed);
	}

	private void PlantPossessing(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		Debug.Log($"Fox Behaviour: Plant Possessing - [{e}]");
	}

	private void PlantPossessed(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		//	More insistent
		//	Run to the bug sprayer
		Debug.Log($"Fox Behaviour: Plant Possessed - [{e}]");
	}

	private void SpiritWaveSpawned(object sender, Spirit[] e)
	{
		//	The fox might not do much here
		//	Could also use the camera for some of it
		Debug.Log($"Fox Behaviour: Spirit Wave Spawned - [{e.Length}]");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.IsLayer(CommonTypes.Layers.Player))
		{
			SetIdle();
		}
	}

	private void SetIdle()
	{
		_state = FoxState.Idle;
		//	TODO: A bit abrupt
		_currentSpeed = 0f;

		//	TODO: Test Alert animation
		_animator.SetTrigger(CommonTypes.AnimatorActions.Alert);
	}
}
