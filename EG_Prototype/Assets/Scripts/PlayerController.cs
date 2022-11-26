using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 2.0f;

    [SerializeField]
    private float _turnSpeed = 10.0f;

    private Vector2 _moveDirection = Vector2.zero;
    private bool _interactionPressed;

    [SerializeField]
    private float _interactionRadius = 2.0f;
    private List<GameObject> _interactableGOsInRadius;

    public void OnMove(InputAction.CallbackContext context)
    {
        //  Only act on completed movement key events
        if (context.performed || context.canceled)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _interactionPressed = true;

                //interactables are sorted by distance from player so interacting with the first one will interact with the closest
                if (_interactableGOsInRadius.Count > 0)
                {
                    _interactableGOsInRadius[0].GetComponent<IInteractable>().OnPlayerInteract(this);
                }

                break;
            case InputActionPhase.Canceled:
                _interactionPressed = false;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _interactableGOsInRadius = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_maxSpeed * Time.deltaTime * new Vector3(_moveDirection.x, 0, _moveDirection.y));

        //	TODO: Rotate the character relative to its axis in the direction of the movement
        //		transform.Rotate(_turnSpeed * Time.deltaTime * new Vector3(0, _moveDirection.x, 0));

        //remove null or inactive interactables and order them by distance from character
        if (_interactableGOsInRadius.Count > 0)
        {
            _interactableGOsInRadius = _interactableGOsInRadius.Where(i => i != null && i.activeInHierarchy).OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).ToList();
        }
    }

    //uses OnTriggerStay rather than OnTriggerEnter as OnTriggerEnter is not called is a gameObject is set to active within the collider
    void OnTriggerStay(Collider other)
    {
        var interactable = other.gameObject.GetComponent<IInteractable>();
        if (interactable != null && interactable.IsInteractable() && !_interactableGOsInRadius.Contains(other.gameObject))
        {
            _interactableGOsInRadius.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        _interactableGOsInRadius.Remove(other.gameObject);
    }
}
