using Boo.Lang;
using UnityEngine;

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
	private float _maxAnimationSpeed = 4.0f;

	[Header("Interaction")]
	//	The player model's root has a bad scale factor, correct for it here.
	[SerializeField]
	private float _scaleCorrection = 100.0f;

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
	private float _movementSpeed;
	private List<IPickUp> _pickups = new List<IPickUp>();
	private GameObject _heldObject = null;

	private bool IsMoving => _moveDirection.sqrMagnitude > 0;

	// Start is called before the first frame update
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_animator = GetComponentInChildren<Animator>();

		_inputEventHandler.Movement += OnMovement;
		_inputEventHandler.InteractionPressed += OnInteractionPressed;
		_inputEventHandler.InteractionReleased += OnInteractionReleased;
	}

	private void OnInteractionPressed(object sender, float e)
	{
		Debug.Log($"PlayerController.InteractionPressed: {_pickups.Count}");

		if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null)
		{
			if (_heldObject.GetComponent<IPickUp>().CanBeDropped)
			{
				AudioController.PlayAudio(_audioSource, _putDownAudio);
				_heldObject.GetComponent<IPickUp>().OnDrop();
				_heldObject.transform.SetParent(null);
				_heldObject = null;
				return;
			}
			else
			{
				return;
			}
		}
		else if (_pickups.Count > 0)
		{
			AudioController.PlayAudio(_audioSource, _pickUpAudio);
			_heldObject = _pickups[0].PickUpObject();
			_heldObject.GetComponent<IPickUp>().OnPickUp();

			//	TODO: Store the orientation of the object, and replace it with the same orientation again

			Debug.Log($"Before SetParent: {_heldObject.transform.position} | {_heldObject.transform.localScale} | {_heldObject.transform.rotation}");
			_heldObject.transform.SetParent(_heldObjectTransform, false);
			Debug.Log($"After SetParent: {_heldObject.transform} | {_heldObject.transform.localScale} | {_heldObject.transform.rotation}");
			_pickups.RemoveAt(0);
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
		/*	TODO: Review the movement again. There's physics rotation and model rotation being applied, 
		 *		as well as shifting the direction of the force applied and then force is being nullified,
		 *		depending on keypresses. It feels like there are redundant actions.
		 */
		//	If there are any keys down, we should move
		if (IsMoving)
		{
			// Rotate player model in direction of movement
			var direction = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, _moveDirection), 0);
			var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);
			//_playerModel.transform.rotation = turnDirection.normalized;
			transform.rotation = turnDirection.normalized;

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
		if (other.TryGetComponent<IPickUp>(out var pickup))
		{
			_pickups.Add(pickup);
			Debug.Log($"PlayerController.OnTriggerEnter:{other.gameObject.name} - {pickup.CanBePickedUp}: {_pickups.Count}");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<IPickUp>(out var pickup))
		{
			_pickups.Remove(pickup);
			Debug.Log($"PlayerController.OnTriggerExit: {other.gameObject.name} - {pickup.CanBePickedUp}: {_pickups.Count} ");
		}
	}
}
