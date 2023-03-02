using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

	[Header("Pick Up UI")]
	[SerializeField]
	private PickUpIndicator _pickUpIndicator;

	[SerializeField]
	private PickUpIndicator _carryIndicator;

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

	[SerializeField]
	private ScriptableAudioClip _digAudio;

	private Vector2 _moveDirection = Vector2.zero;
	private Rigidbody _rb;
	private Animator _animator;
	private Camera _camera;
	private float _movementSpeed;

	// HashSet to prevent duplicates
	private HashSet<IPickUp> _pickups = new HashSet<IPickUp>();
	private HashSet<IInteractable> _interactables = new HashSet<IInteractable>();
	private IPickUp _heldObject = null;
	private PickUpSpawnerBase _spawner;
	private float _carryIndicatorLocalY;
	private bool _canMove = true;   //	Set this when an animation is playing.
	private bool _digAnimationPlaying = false;

	private bool IsMoving => _moveDirection.sqrMagnitude > 0;

	private void OnInteractionPressed(object sender, float e)
	{
		if (_canMove)
		{
			switch (_heldObject == null)
			{
				case false when _heldObject.CanBeDropped:
					DropObject(false, true);
					break;
				case true when _pickups.Count > 0 || _spawner != null:
					StartCoroutine(PickUpObjectCoroutine());
					break;
				default:
					break;
			}
		}
	}

	private IEnumerator PickUpObjectCoroutine()
	{
		_heldObject = PickupCorrectObject();
		//	It is possible that the object despawned, so make sure there is an object to pick up.
		if (_heldObject != null)
		{
			AudioController.PlayAudio(_audioSource, _pickUpAudio);

			// Reset local position of carry indicator to account for shovel bounce implementation
			_carryIndicator.transform.localPosition = new Vector3(_carryIndicator.transform.localPosition.z, _carryIndicatorLocalY, _carryIndicator.transform.localPosition.z);

			//	Enable Icon to indicate carried object. Must be called before _heldObject.OnPickUP to prevent carry indicator colours being overwritten
			SetCarryIndicator(true, _heldObject);
			_heldObject.OnPickUp(_heldObjectTransform);

			//	!! Animation clip uses left hand !! - Assuming _spawner will be null if the player is picking up something from the ground.
			if (_heldObject.PlayPickUpAnimation && (_spawner == null || _spawner.PlayPickUpAnimation))
			{
				_canMove = false;
				//	Play the pickup animation and wait for it to complete
				yield return _animator.TriggerAndWaitForAnimation(CommonTypes.AnimatorActions.PickUp);
				_canMove = true;
			}
		}
	}

	private IPickUp GetClosestPickup()
	{
		return _pickups.Where(p => p.CanBePickedUp).OrderBy(p => Vector3.Distance(transform.position, p.Transform.position)).FirstOrDefault();
	}

	private IPickUp PickupCorrectObject()
	{
		IPickUp closestPickUp = GetClosestPickup();
		switch (closestPickUp == null)
		{
			case true when _spawner != null:
				return _spawner.SpawnPickUp();
			case false when _spawner == null:
				return closestPickUp;
			case false when _spawner != null:
				return Vector3.Distance(_spawner.transform.position, transform.position) < Vector3.Distance(closestPickUp.Transform.position, transform.position) ? _spawner.SpawnPickUp() : closestPickUp;
			default:
				return null;
		}
	}

	private void DropObject(bool destroy, bool playAudio)
	{
		if (playAudio)
		{
			AudioController.PlayAudio(_audioSource, _putDownAudio);
		}

		StartCoroutine(DropObjectCoroutine(destroy));
	}

	private IEnumerator DropObjectCoroutine(bool destroy)
	{
		if (_heldObject?.PlayDropAnimation == true)
		{
			_canMove = false;
			yield return _animator.TriggerAndWaitForAnimation(CommonTypes.AnimatorActions.Drop);
			_canMove = true;
		}

		_heldObject?.OnDrop(destroy);
		_heldObject = null;
		SetCarryIndicator(false);
	}

	private void OnInteractionReleased(object sender, float e)
	{
		//	Do nothing for now
	}

	private void OnMovement(object sender, Vector2 e)
	{
		_moveDirection = e;
	}

	private void SetPickUpIndicator(bool active, Vector3? position = null)
	{
		if (_pickUpIndicator.Active != active)
		{
			_pickUpIndicator.SetActive(active);
		}
		if (position != null)
		{
			_pickUpIndicator.transform.position = position.Value;
		}
		//_pickUpIndicator.transform.LookAt(_camera.transform.position.ZeroY());		
	}

	private void SetCarryIndicator(bool enabled, IPickUp held = null)
	{
		if (held != null)
		{
			_carryIndicator.SetIcon(held.CarryIcon);
			_carryIndicator.SetIconColor(held.CarryIconBaseColor);
			_carryIndicator.SetSecondaryIcon(held.CarryIconSecondary);
			_carryIndicator.SetSecondaryIconColor(held.CarryIconSecondaryColor);
		}

		_carryIndicator.SetActive(enabled);
	}

	private void SubscribeToEvents()
	{
		_inputEventHandler.Movement += OnMovement;
		_inputEventHandler.InteractionPressed += OnInteractionPressed;
		_inputEventHandler.InteractionReleased += OnInteractionReleased;

		Shovel.DigEvent += OnDig;
	}

	private void UnsubscribeFromEvents()
	{
		//	Disconnect event handlers
		_inputEventHandler.Movement -= OnMovement;
		_inputEventHandler.InteractionPressed -= OnInteractionPressed;
		_inputEventHandler.InteractionReleased -= OnInteractionReleased;

		Shovel.DigEvent -= OnDig;
	}

	private void OnDestroy()
	{
		UnsubscribeFromEvents();
	}

	// Start is called before the first frame update
	private void Start()
	{
		_animator = GetComponentInChildren<Animator>();
		Assert.IsNotNull(_animator, Utility.AssertNotNullMessage(nameof(_animator)));
		if (!TryGetComponent(out _rb))
			Assert.IsNotNull(_rb, Utility.AssertNotNullMessage(nameof(_rb)));

		//	Fetch the main camera
		_camera = Camera.main;

		_carryIndicatorLocalY = _carryIndicator.transform.localPosition.y;

		SubscribeToEvents();
	}

	// Update is called once per frame
	private void Update()
	{
		//	Clear any PickUps that have despawned from the list
		if (_pickups.Count > 0 && _pickups.Where(p => p.Despawned).ToList() is var pickupList && pickupList.Count() > 0)
		{
			foreach (var p in pickupList)
				_pickups.Remove(p);
		}

		switch (_heldObject)
		{
			case IInteractor interactor when _interactables.Where(i => i.CanInteractWith(interactor)).ToList() is var interactables && interactables.Count > 0:
				{
					var interactable = interactables
						  .OrderBy(i => Vector3.Distance(transform.position, i.Transform.position))
						  .FirstOrDefault();
					interactable.OnInteractWith(interactor);
					interactor.OnInteract(interactable);
					var destroyInteractable = interactable.DestroyOnInteract(interactor);
					var destroyInteractor = interactor.DestroyAfterInteract(interactable);
					if (destroyInteractable)
					{
						_interactables.Remove(interactable);
						interactable.DestroyInteractable();
					}
					if (destroyInteractor)
					{
						if (interactor is IPickUp)
						{
							_pickups.Remove(interactor as IPickUp);
						}
						interactor.DestroyInteractor();
						_heldObject = null;
						SetCarryIndicator(false);
					}
					break;
				}
			default:
				break;
		}

		var closestPickUp = GetClosestPickup();
		switch (closestPickUp == null)
		{
			case true when _spawner != null && _heldObject == null:
				// Indicator at spawner
				SetPickUpIndicator(true, _spawner.IndicatorPosition);
				break;
			case false when _spawner == null && _heldObject == null:
				// Indicator at closest pick up
				SetPickUpIndicator(true, closestPickUp.IndicatorPostion);
				break;
			case false when _spawner != null && _heldObject == null:
				// Indicator at closest between pick up and spawner
				SetPickUpIndicator(
					true,
					Vector3.Distance(_spawner.transform.position, transform.position) < Vector3.Distance(closestPickUp.Transform.position, transform.position)
						? _spawner.IndicatorPosition
						: closestPickUp.IndicatorPostion
				);
				break;
			default:
				// Disable indicator
				SetPickUpIndicator(false);
				break;
		}

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
		//	If there are any keys down, we should move, unless a stationary animation is playing
		if (_canMove && IsMoving)
		{
			// Calculate camera's rotation about the y axis
			Quaternion cameraRotation = Quaternion.AngleAxis(_camera.transform.eulerAngles.y, Vector3.up);

			// Apply camera rotation to move direction
			Vector3 moveV3 = cameraRotation * new Vector3(_moveDirection.x, 0, _moveDirection.y);

			// Vector2 representation of player input adjusted for camera rotation
			Vector2 moveV2 = new Vector2(moveV3.x, moveV3.z);

			// Calculate and apply player character rotations based on moveV2
			var direction = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, moveV2), 0);
			var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);
			_rb.MoveRotation(turnDirection.normalized);

			// Apply velocity change to rigidbody in desired direction of movement
			_rb.AddForce(_acceleration * Time.deltaTime * moveV3, ForceMode.VelocityChange);

			// Calculate rigidbody velocity rotated to align with player input
			var adjustedVelocity = cameraRotation * -_rb.velocity;

			// Adjust to ensure diagonal movement is exactly in direction desired without sliding
			if (_moveDirection.x != 0 && _moveDirection.y != 0)
			{
				adjustedVelocity.x = _moveDirection.x;
				adjustedVelocity.z = _moveDirection.y;
				adjustedVelocity = adjustedVelocity.normalized * _rb.velocity.magnitude;
			}

			// Adjust to remove movement in a direction that is not desired
			if (_moveDirection.x == 0 || (_moveDirection.x < 0 && adjustedVelocity.x > 0) || (_moveDirection.x > 0 && adjustedVelocity.x < 0))
			{
				adjustedVelocity.x = 0;
			}
			if (_moveDirection.y == 0 || (_moveDirection.y < 0 && adjustedVelocity.y > 0) || (_moveDirection.y > 0 && adjustedVelocity.y < 0))
			{
				adjustedVelocity.z = 0;
			}

			// Apply velocity adjustments to the rigidbody
			_rb.velocity = cameraRotation * adjustedVelocity;

			// Clamp the speed
			if (_rb.velocity.magnitude > _maxSpeed)
			{
				_rb.velocity = _rb.velocity.normalized * _maxSpeed;
			}
		}
		else
		{
			//	Set velocity to zero
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.TryGetComponent<IPickUp>(out var pickup) && !_pickups.Contains(pickup))
		{
			if (pickup.CanBePickedUp)
			{
				_pickups.Add(pickup);
			}
			if (other.TryGetComponent<ICombinable>(out var combinable))
			{
				// Attempts to remove event before adding it. This ensures the event is only subscribed to once by this
				combinable.CombineProgress -= CombineProgress;
				combinable.CombineProgress += CombineProgress;
			}
		}
		if (other.TryGetComponent<PickUpSpawnerBase>(out var spawner) && spawner != _spawner)
		{
			if (_spawner == null || Vector3.Distance(transform.position, spawner.transform.position) < Vector3.Distance(transform.position, _spawner.transform.position))
			{
				_spawner = spawner;
			}
		}
		if (other.TryGetComponent<IInteractable>(out var interactable) && !_interactables.Contains(interactable))
		{
			_interactables.Add(interactable);
		}
	}

	private void CombineProgress(object sender, float e)
	{
		var pickup = (IPickUp)sender;
		if (pickup == _heldObject)
		{
			_carryIndicator.SetIconColor(Color.Lerp(pickup.CarryIconBaseColor.MaxAlpha(), pickup.CarryIconBaseColor.ZeroAlpha(), e / ((ICombinable)sender).CombinationThreshold));
			_carryIndicator.SetSecondaryIconColor(Color.Lerp(pickup.CarryIconSecondaryColor.ZeroAlpha(), pickup.CarryIconSecondaryColor.MaxAlpha(), e / ((ICombinable)sender).CombinationThreshold));
		}
	}

	private void OnDig(object sender, IPickUp e)
	{
		if (e == _heldObject)
		{
			StartCoroutine(DiggingCoroutine());

			var bounce = Mathf.Sin(Time.time * 8);
			var prevBounce = Mathf.Sin((Time.time - Time.deltaTime) * 8);
			_carryIndicator.transform.position += 0.015f * bounce * Vector3.up;

			if (bounce <= -0.999f && bounce < prevBounce)
			{
				AudioController.PlayAudio(_audioSource, _digAudio);
			}
		}
	}

	private IEnumerator DiggingCoroutine()
	{
		if (!_digAnimationPlaying)
		{
			_digAnimationPlaying = true;
			_canMove = false;
			//	Make sure this is triggered only once.
			yield return _animator.TriggerAndWaitForAnimation(CommonTypes.AnimatorActions.Digging);
			_digAnimationPlaying = false;
			_canMove = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<IPickUp>(out var pickup))
		{
			_pickups.Remove(pickup);
		}
		if (other.TryGetComponent<PickUpSpawnerBase>(out var spawner))
		{
			if (spawner == _spawner)
			{
				_spawner = null;
			}
		}
		if (other.TryGetComponent<IInteractable>(out var interactable))
		{
			_interactables.Remove(interactable);
		}
	}
}
