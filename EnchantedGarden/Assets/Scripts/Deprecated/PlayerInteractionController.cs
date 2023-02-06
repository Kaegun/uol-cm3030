using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField]
    private ScriptableInputEventHandler _input;

    [SerializeField]
    private float _interactionRadius = 2.0f;

    [SerializeField]
    private Transform _heldObjectTransform;

    private GameObject _heldObject = null;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private ScriptableAudioClip _pickUpAudio;

    [SerializeField]
    private ScriptableAudioClip _putDownAudio;

    //public void OnInteraction(InputAction.CallbackContext context)
    //{
    /*switch (context.phase)
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

			var interactables = Physics.OverlapSphere(transform.position, _interactionRadius).
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
	}*/
    //}

    //public void OnPickUp(InputAction.CallbackContext context)
    //{
    //	switch (context.phase)
    //	{
    //		case InputActionPhase.Started:
    //			if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null && !_heldObject.GetComponent<IPickUp>().CanBeDropped)
    //			{
    //				break;
    //			}

    //			if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null && _heldObject.GetComponent<IPickUp>().CanBeDropped)
    //			{
    //				_heldObject.GetComponent<IPickUp>().OnDrop();
    //				_heldObject = null;
    //				break;
    //			}

    //			var pickUps = Physics.OverlapSphere(transform.position, _interactionRadius)
    //				.Where(p => p.GetComponent<IPickUp>() != null && p.GetComponent<IPickUp>().CanBePickedUp)
    //				.Select(p => p.gameObject)
    //				.OrderBy(p => Vector3.Distance(p.transform.position, transform.position))
    //				.ToList();

    //			if (pickUps.Count > 0)
    //			{
    //				AudioController.GetInstance().PlayPickUp();
    //				_heldObject = pickUps[0].GetComponent<IPickUp>().PickUpObject();
    //				_heldObject.GetComponent<IPickUp>().OnPickUp();
    //			}
    //			break;
    //		case InputActionPhase.Canceled:
    //			break;
    //	}
    //}

    //	Start gets called once when the script is loaded
    private void Start()
    {
        _input.InteractionPressed += InteractionPressed;
    }

    private void InteractionPressed(object sender, float e)
    {
        if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null && !_heldObject.GetComponent<IPickUp>().CanBeDropped)
        {
            return;
        }

        if (_heldObject != null && _heldObject.GetComponent<IPickUp>() != null && _heldObject.GetComponent<IPickUp>().CanBeDropped)
        {
            AudioController.PlayAudio(_audioSource, _putDownAudio);
            _heldObject.GetComponent<IPickUp>().OnDrop();
            _heldObject = null;
            return;
        }

        var pickUps = Physics.OverlapSphere(transform.position, _interactionRadius)
            .Where(p => p.GetComponent<IPickUp>() != null && p.GetComponent<IPickUp>().CanBePickedUp)
            .Select(p => p.gameObject)
            .OrderBy(p => Vector3.Distance(p.transform.position, transform.position))
            .ToList();

        if (pickUps.Count > 0)
        {
            AudioController.PlayAudio(_audioSource, _pickUpAudio);
            _heldObject = pickUps[0].GetComponent<IPickUp>().PickUpObject();
            _heldObject.GetComponent<IPickUp>().OnPickUp();
            _heldObject.transform.SetParent(transform.parent, false);
        }
    }

    //	Update is called once per frame
    private void Update()
    {
        //  Check whether this works on a pickup
        ////	TODO: We can attach to the player
        ////	Move held object with the player
        //if (_heldObject != null)
        //{
        //    _heldObject.transform.position = _heldObjectTransform.position;
        //}
    }
}
