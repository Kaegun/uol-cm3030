using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhysicsController : MonoBehaviour
{
	[SerializeField]
	private float _acceleration = 20.0f;

	[SerializeField]
	private float _maxSpeed = 6.0f;

	//	Degrees to turn per second
	[SerializeField]
	private float _turnSpeed = 90.0f;

	[SerializeField]
	private float _interactionRadius = 2.0f;

	//	Should probably not be here
	[SerializeField]
	private float _maxAnimationSpeed = 4.0f;

	private Vector2 _moveDirection = Vector2.zero;
	private Rigidbody _rb;
	private Animator _animator;
	private float _movementSpeed;

	//private bool _interactionPressed;
	//private List<GameObject> _interactableGOsInRadius;
	//private GameObject _heldObject = null;

	private bool IsMoving => _moveDirection.sqrMagnitude > 0;

	public void OnMove(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				break;
			case InputActionPhase.Performed:
			case InputActionPhase.Canceled:
				_moveDirection = context.ReadValue<Vector2>();
				break;
		}
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				//_interactionPressed = true;

				//if (_heldObject != null)
				//{
				//	_heldObject.GetComponent<IInteractable>().OnPlayerInteract(this);
				//	break;
				//}

				////interactables are sorted by distance from player so interacting with the first one will interact with the closest
				//if (_interactableGOsInRadius.Count > 0)
				//{
				//	_interactableGOsInRadius[0].GetComponent<IInteractable>().OnPlayerInteract(this);
				//}

				break;
			case InputActionPhase.Canceled:
				//_interactionPressed = false;
				break;
		}
	}

	public void OnFire2(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				break;
		}
	}

	public void Pickup(GameObject obj)
	{
		//_heldObject = obj;
	}

	public void Drop(GameObject obj)
	{
		//_heldObject = null;
	}

	// Start is called before the first frame update
	private void Start()
	{
		//_interactableGOsInRadius = new List<GameObject>();
		_rb = GetComponent<Rigidbody>();
		_animator = GetComponentInChildren<Animator>();
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

		//transform.Translate(_maxSpeed * Time.deltaTime * new Vector3(_moveDirection.x, 0, _moveDirection.y));

		//	TODO: Rotate the character relative to its axis in the direction of the movement
		//		transform.Rotate(_turnSpeed * Time.deltaTime * new Vector3(0, _moveDirection.x, 0));

		// remove null or inactive interactables and order them by distance from character
		// TODO: Add highlight to closest interactable
		//if (_interactableGOsInRadius.Count > 0)
		//{
		//	_interactableGOsInRadius = _interactableGOsInRadius.Where(i => i != null && i.activeInHierarchy).OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).ToList();
		//}

		//// move held object with the player
		//if (_heldObject != null)
		//{
		//	_heldObject.transform.position = transform.position + new Vector3(0.5f, 0f, 0.5f);
		//}
	}

	private void FixedUpdate()
	{
		Debug.Log($"Move Direction: {_moveDirection}");

		//	If there are any keys down, we should move
		if (IsMoving)
		{
			Debug.Log("Moving");
			var direction = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, _moveDirection), 0);

			var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);
			_rb.MoveRotation(turnDirection.normalized);
			//	Check whether the direction needs to be inverted, and bleed off the speed
			var dot = Vector3.Dot(new Vector3(_moveDirection.x, 0, _moveDirection.y), transform.forward);
			if (!Mathf.Approximately(dot, 1.0f))
			{
				_rb.velocity *= dot;
			}
			_rb.AddForce(_acceleration * Time.deltaTime * transform.forward, ForceMode.VelocityChange);

			//	Clamp the speed
			if (_rb.velocity.magnitude > _maxSpeed)
			{
				_rb.velocity = _rb.velocity.normalized * _maxSpeed;
			}
		}
		else
		{
			Debug.Log("stopping");
			//	Set velocity to zero
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;

		}
	}

	//uses OnTriggerStay rather than OnTriggerEnter as OnTriggerEnter is not called is a gameObject is set to active within the collider
	private void OnTriggerStay(Collider other)
	{
		//var interactable = other.gameObject.GetComponent<IInteractable>();
		//if (interactable != null && interactable.IsInteractable() && !_interactableGOsInRadius.Contains(other.gameObject))
		//{
		//	_interactableGOsInRadius.Add(other.gameObject);
		//}
	}

	private void OnTriggerExit(Collider other)
	{
		//_interactableGOsInRadius.Remove(other.gameObject);
	}
}
