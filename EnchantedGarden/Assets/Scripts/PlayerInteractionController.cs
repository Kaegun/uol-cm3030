using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField]
    private float _interactionRadius = 2.0f;

    [SerializeField]
    private Transform _heldObjectTransform;

    //private bool _interactionPressed;
    //private List<GameObject> _interactableGOsInRadius;
    private GameObject _heldObject = null;    

    public void OnInteraction(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (_heldObject != null)
                {
                    if (_heldObject.GetComponent<IInteractable>() != null && _heldObject.GetComponent<IInteractable>().CanBeInteractedWith())
                    {
                        _heldObject.GetComponent<IInteractable>().OnPlayerInteract(this);
                    }
                    break;
                }

                var interactables = Physics.OverlapSphere(transform.position, 2f).
                    Where(i => i.GetComponent<IInteractable>() != null && i.GetComponent<IInteractable>().CanBeInteractedWith()).
                    Select(i => i.gameObject).
                    OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).
                    ToList();

                if (interactables.Count > 0)
                {
                    interactables[0].GetComponent<IInteractable>().OnPlayerInteract(this);
                }
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null && _heldObject.GetComponent<IPickUp>().CanBeDropped())
                {
                    _heldObject.GetComponent<IPickUp>().OnDrop();
                    _heldObject = null;
                    break;
                }

                var pickUps = Physics.OverlapSphere(transform.position, 2f).
                    Where(p => p.GetComponent<IPickUp>() != null && p.GetComponent<IPickUp>().CanBePickedUp()).
                    Select(p => p.gameObject).
                    OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).
                    ToList();

                if (pickUps.Count > 0)
                {
                    AudioController.GetInstance().PlayPickUp();
                    _heldObject = pickUps[0].GetComponent<IPickUp>().PickUpObject();
                    _heldObject.GetComponent<IPickUp>().OnPickUp();
                }
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }    

    // Start is called before the first frame update
    private void Start()
    {
        //_interactableGOsInRadius = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        //	TODO: Rotate the character relative to its axis in the direction of the movement
        //		transform.Rotate(_turnSpeed * Time.deltaTime * new Vector3(0, _moveDirection.x, 0));

        // remove null or inactive interactables and order them by distance from character
        // TODO: Add highlight to closest interactable
        //if (_interactableGOsInRadius.Count > 0)
        //{
        //	_interactableGOsInRadius = _interactableGOsInRadius.Where(i => i != null && i.activeInHierarchy).OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).ToList();
        //}

        // move held object with the player
        if (_heldObject != null)
        {
            _heldObject.transform.position = _heldObjectTransform.position;
        }
    }   
}
