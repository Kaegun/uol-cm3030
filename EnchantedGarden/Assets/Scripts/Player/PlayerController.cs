﻿using System.Collections.Generic;
using System.Linq;
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
    // HashSet to prevent duplicates
    private HashSet<IPickUp> _pickups = new HashSet<IPickUp>();
    private HashSet<IInteractable> _interactables = new HashSet<IInteractable>();
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

        switch (_heldObject == null)
        {
            case false when _heldObject.CanBeDropped:
                HandleDropObject();
                break;
            case true when _pickups.Count > 0 || _spawner != null:
                AudioController.PlayAudio(_audioSource, _pickUpAudio);
                _heldObject = PickupCorrectObject();
                _heldObject.OnPickUp(_heldObjectTransform);
                break;
            default:
                break;
        }

        // Refactor into switch statement?
        //if (_heldObject != null)
        //{
        //    if (_heldObject.CanBeDropped)
        //    {
        //        HandleDropObject();
        //        // TODO: Delete
        //        //if (_cauldron != null)
        //        //{
        //        //    HandleCauldronInteraction();
        //        //    return;
        //        //}
        //        //else
        //        //{
        //        //    DropObject(false, true);
        //        //    return;
        //        //}
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}
        //else if (_pickups.Count > 0)
        //{
        //    AudioController.PlayAudio(_audioSource, _pickUpAudio);
        //    _heldObject = PickupCorrectObject();
        //}
        //else if (_spawner != null)
        //{
        //	_heldObject = _spawner.SpawnPickUp();
        //	_heldObject.OnPickUp(_heldObjectTransform);
        //}
    }

    private IPickUp PickupCorrectObject()
    {
        IPickUp closestPickUp = _pickups.OrderBy(p => Vector3.Distance(transform.position, p.Transform.position)).FirstOrDefault();
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

    private void HandleDropObject()
    {
        switch (_heldObject)
        {
            case Log _ when _cauldron != null:
                _cauldron.AddLog();
                DropObject(true, false);
                break;
            case Ingredient _ when _cauldron != null:
                _cauldron.AddIngredient();
                DropObject(true, false);
                break;
            case PesticideSpray pesticideSpray when _cauldron != null:
                _cauldron.FillPesticideSpray(pesticideSpray);
                break;
            default:
                DropObject(false, true);
                break;
        }
    }

    private void DropObject(bool destroy, bool playAudio)
    {
        if (playAudio)
        {
            AudioController.PlayAudio(_audioSource, _putDownAudio);
        }

        _heldObject?.OnDrop(destroy);
        _heldObject = null;
    }

    //private void HandleCauldronInteraction()
    //{
    //    Debug.Log($"{nameof(HandleCauldronInteraction)} - ({_heldObject.GetType()})");
    //
    //    switch (_heldObject)
    //    {
    //        case Log _:
    //            //	Drop the log to increase the fire
    //            _cauldron.AddLog();
    //            DropObject(true, false);
    //            break;
    //        case Ingredient _:
    //            //	Mixing ingredients
    //            _cauldron.AddIngredient();
    //            DropObject(true, false);
    //            break;
    //        case PesticideSpray pesticideSpray:
    //            //	Fill potion bottle to dispel spirits
    //            _cauldron.FillPesticideSpray(pesticideSpray);
    //            break;
    //        default:
    //            break;
    //    }
    //}

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
        switch (_heldObject)
        {
            // When held object is a shovel and there are nearby interactable that can be interacted with by a shovel
            case Shovel shovel when _interactables.Select(i => i as IInteractable<Shovel>).Where(i => i.CanInteractWith(shovel)).ToList().Count > 0:
                {
                    var interactable = _interactables.Select(i => i as IInteractable<Shovel>)
                        .Where(i => i.CanInteractWith(shovel))
                        .OrderBy(i => Vector3.Distance(transform.position, i.Transform.position))
                        .FirstOrDefault();
                    interactable.OnInteractWith(shovel);
                    break;
                }
            // When held object is pesticide spray and there are nearby interactable that can be interacted with by pesticide spray
            case PesticideSpray spray when _interactables.Select(i => i as IInteractable<PesticideSpray>).Where(i => i.CanInteractWith(spray)).ToList().Count > 0:
                {
                    Debug.Log("Using pesticide spray");
                    var interactable = _interactables.Select(i => i as IInteractable<PesticideSpray>)
                        .Where(i => i.CanInteractWith(spray))
                        .OrderBy(i => Vector3.Distance(transform.position, i.Transform.position))
                        .FirstOrDefault();
                    if (interactable is Spirit spirit)
                    {
                        _interactables.Remove(spirit);
                    }
                    interactable?.OnInteractWith(spray);
                }
                break;
            //case ICombinable combinable when _interactables.Select(i => i as IInteractable<ICombinable>).Where(i => i.CanInteractWith(combinable)).FirstOrDefault() is var interactable && interactable != null:
            //    //interactables.OrderBy(i => Vector3.Distance(transform.position, i.Transform.position)).FirstOrDefault().OnInteractWith(spray);
            //    interactable.OnInteractWith(combinable);
            //    break;
            default:
                break;
        }

        if (_pickups.Count > 0 || _spawner != null)
        {
            // TODO: Highlight closest pickup/spawner
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
        //	If there are any keys down, we should move
        if (IsMoving)
        {
            // TODO: This is a bad way to do this and needs fixing
            // Rotate player model in direction of movement - adjust for camera
            // Calculate vector from camera to player
            Vector3 camToPlayer = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(_camera.transform.position.x, 0, _camera.transform.position.z);
            // Calculate angle of vector from camera to player about the y axis
            float camToPlayerAngle = Vector3.SignedAngle(Vector3.forward, camToPlayer, Vector3.up);
            // Rotate move direction vector by angle from camera to player
            Vector3 moveV3 = (Quaternion.Euler(0, camToPlayerAngle, 0) * new Vector3(_moveDirection.x, 0, _moveDirection.y)).normalized;
            // Create new 2d vector of movement used in place of _moveDirection
            Vector2 moveV2 = new Vector2(moveV3.x, moveV3.z);

            // Calculate and apply rotations based on moveV2
            var direction = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, moveV2), 0);
            var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);

            _rb.MoveRotation(turnDirection.normalized);

            // Apply velocity change to rigidbody in direction of movement
            _rb.AddForce(_acceleration * Time.deltaTime * moveV3, ForceMode.VelocityChange);

            // Clamp the speed
            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }

            // If player has no input in a direction set the velocity for that direction to 0
            if (moveV2.x == 0)
            {
                _rb.velocity = new Vector3(0, _rb.velocity.y, _rb.velocity.z);
            }
            if (moveV2.y == 0)
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
        //Debug.Log($"PlayerController.OnTriggerEnter: {other.name}");
        if (other.TryGetComponent<IPickUp>(out var pickup))
        {
            if (pickup.CanBePickedUp)
                _pickups.Add(pickup);
            //Debug.Log($"PlayerController.OnTriggerEnter:{other.gameObject.name} - {pickup.CanBePickedUp}: {_pickups.Count}");
        }
        else if (other.TryGetComponent<PickUpSpawnerBase>(out var spawner))
        {
            _spawner = spawner;
        }
        else if (other.TryGetComponent<Cauldron>(out var cauldron))
        {
            _cauldron = cauldron;
        }
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            _interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"PlayerController.OnTriggerExit: {other.name}");
        if (other.TryGetComponent<IPickUp>(out var pickup))
        {
            _pickups.Remove(pickup);
            //Debug.Log($"PlayerController.OnTriggerExit: {other.name} - {pickup.CanBePickedUp}: {_pickups.Count}");
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
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            _interactables.Remove(interactable);
        }
    }
}
