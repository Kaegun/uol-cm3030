using Boo.Lang;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	[Header("Input")]
	[SerializeField]
	private ScriptableInputEventHandler _inputEventHandler;

	[SerializeField]
	private float _acceleration = 20.0f;

	[SerializeField]
	private float _maxSpeed = 6.0f;

	//	Degrees to turn per second
	[SerializeField]
	private float _turnSpeed = 90.0f;

	//	Maximum speed set on the animator
	[SerializeField]
	private float _maxAnimationSpeed = 6.0f;

	[Header("Interaction")]
	[SerializeField]
	private Transform _heldObjectTransform;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private ScriptableAudioClip _pickUpAudio;

	[SerializeField]
	private ScriptableAudioClip _putDownAudio;

	private Vector2 _moveDirection = Vector2.zero;
	private Rigidbody _rb;
	private Animator _animator;
	private Camera _camera;
	private float _movementSpeed;
	private List<IPickUp> _pickups = new List<IPickUp>();
	private IPickUp _heldObject = null;
	private PickUpSpawnerBase _spawner;
	private Cauldron _cauldron;

	private bool IsMoving => _moveDirection.sqrMagnitude > 0;

	// Start is called before the first frame update
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_animator = GetComponentInChildren<Animator>();
		//	TODO: I know there's some issues with this approach
		_camera = Camera.main;

		_inputEventHandler.Movement += OnMovement;
		_inputEventHandler.InteractionPressed += OnInteractionPressed;
		_inputEventHandler.InteractionReleased += OnInteractionReleased;
	}

	private void OnInteractionPressed(object sender, float e)
	{
		Debug.Log($"PlayerController.InteractionPressed: Pickups: {_pickups.Count} | Cauldron: {_cauldron != null} | HeldObject: {_heldObject != null}");

		if (_heldObject != null)
		{
			if (_heldObject.CanBeDropped)
			{
				if (_cauldron != null)
				{
					HandleCauldronInteraction();
					return;
				}
				else
				{
					DropObject(false, true);
					return;
				}
			}
			else
			{
				return;
			}
		}
		else if (_pickups.Count > 0)
		{
			AudioController.PlayAudio(_audioSource, _pickUpAudio);
			_heldObject = PickupCorrectObject();
			_heldObject.OnPickUp(_heldObjectTransform);
		}
		else if (_spawner != null)
		{
			_heldObject = _spawner.SpawnPickUp();
			_heldObject.OnPickUp(_heldObjectTransform);
		}
	}

	private IPickUp PickupCorrectObject()
	{
		var heldObject = _pickups[0];
		_pickups.RemoveAt(0);
		return heldObject;
	}

	private void DropObject(bool destroy, bool playAudio)
	{
		if (playAudio)
			AudioController.PlayAudio(_audioSource, _putDownAudio);

		_heldObject?.OnDrop(destroy);
		_heldObject = null;
	}

	private void HandleCauldronInteraction()
	{
		Debug.Log($"{nameof(HandleCauldronInteraction)} - ({_heldObject.GetType()})");
		if (_heldObject is Log)
		{
			Debug.Log("Dropping a log");
			_cauldron.AddLog();
			DropObject(true, false);
		}
		else if (_heldObject is Ingredient)
		{
			Debug.Log("Mixing ingredients");
			_cauldron.AddIngredient();
			DropObject(true, false);
		}
		else if (_heldObject is PesticideSpray)
		{
			_cauldron.FillPesticideSpray(_heldObject as PesticideSpray);
		}
	}

	private void OnInteractionReleased(object sender, float e)
	{
		//_interactionPressed = e > 0.0;
		//	Do nothing for now
	}

	private void OnMovement(object sender, Vector2 e)
	{
		_moveDirection = e;
	}

	// Update is called once per frame
	private void Update()
	{
		if (IsMoving)
		{
			_movementSpeed = Mathf.Clamp(_movementSpeed + _acceleration * Time.deltaTime, 0.0f, _maxAnimationSpeed);
			if (_animator != null)
			{
				_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _movementSpeed);
			}
		}
		else if (_movementSpeed > 0.0f)
		{
			_movementSpeed = 0.0f;
			_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _movementSpeed);
		}
	}

	private void FixedUpdate()
	{
		//	If there are any keys down, we should move
		if (IsMoving)
		{
			// Rotate player model in direction of movement - adjust for camera
			//	TODO: Apply camera transform to move vector
			var direction = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, _moveDirection), 0);
			var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);

			_rb.MoveRotation(turnDirection.normalized);

			// Apply velocity change to rigidbody in direction of movement
			var moveDirVec3 = new Vector3(_moveDirection.x, 0, _moveDirection.y);
			_rb.AddForce(_acceleration * Time.deltaTime * moveDirVec3, ForceMode.VelocityChange);

			// Clamp the speed
			if (_rb.velocity.magnitude > _maxSpeed)
			{
				_rb.velocity = _rb.velocity.normalized * _maxSpeed;
			}

			// If player has no input in a direction set the velocity for that direction to 0
			if (_moveDirection.x == 0)
			{
				_rb.velocity = new Vector3(0, _rb.velocity.y, _rb.velocity.z);
			}
			if (_moveDirection.y == 0)
			{
				_rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, 0);
			}
		}
		else
		{
			//	Set velocity to zero
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log($"PlayerController.OnTriggerEnter: {other.name}");
		if (other.TryGetComponent<IPickUp>(out var pickup))
		{
			if (pickup.CanBePickedUp)
				_pickups.Add(pickup);
			Debug.Log($"PlayerController.OnTriggerEnter:{other.gameObject.name} - {pickup.CanBePickedUp}: {_pickups.Count}");
		}
		else if (other.TryGetComponent<PickUpSpawnerBase>(out var spawner))
		{
			_spawner = spawner;
		}
		else if (other.TryGetComponent<Cauldron>(out var cauldron))
		{
			_cauldron = cauldron;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log($"PlayerController.OnTriggerExit: {other.name}");
		if (other.TryGetComponent<IPickUp>(out var pickup))
		{
			_pickups.Remove(pickup);
			Debug.Log($"PlayerController.OnTriggerExit: {other.name} - {pickup.CanBePickedUp}: {_pickups.Count}");
		}
		else if (other.TryGetComponent<PickUpSpawnerBase>(out var spawner))
		{
			_spawner = null;
		}
		else if (other.TryGetComponent<Cauldron>(out var _))
		{
			//	Assign cauldron to null
			_cauldron = null;
		}
	}
}
