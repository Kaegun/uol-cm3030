using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
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

	[SerializeField]
	private InputEventScriptableObject _inputEventHandler;

	private Vector2 _moveDirection = Vector2.zero;
	private Rigidbody _rb;
	private Animator _animator;
	private float _movementSpeed;
	private bool _interactionPressed;

	private bool IsMoving => _moveDirection.sqrMagnitude > 0;

	//public void OnMove(InputAction.CallbackContext context)
	//{
	//	switch (context.phase)
	//	{
	//		case InputActionPhase.Started:
	//			break;
	//		case InputActionPhase.Performed:
	//		case InputActionPhase.Canceled:
	//			_moveDirection = context.ReadValue<Vector2>();
	//			break;
	//	}
	//}

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
		_interactionPressed = e > 0.0;
	}

	private void OnInteractionReleased(object sender, float e)
	{
		_interactionPressed = e > 0.0;
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
				_animator.SetFloat("ForwardSpeed", _movementSpeed);
			}
		}
		else if (_movementSpeed > 0.0f)
		{
			_movementSpeed = 0.0f;
			_animator.SetFloat("ForwardSpeed", _movementSpeed);
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
}
