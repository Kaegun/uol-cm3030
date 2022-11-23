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
				break;
			case InputActionPhase.Canceled:
				_interactionPressed = false;
				break;
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(_maxSpeed * Time.deltaTime * new Vector3(_moveDirection.x, 0, _moveDirection.y));

		//	TODO: Rotate the character relative to its axis in the direction of the movement
		//		transform.Rotate(_turnSpeed * Time.deltaTime * new Vector3(0, _moveDirection.x, 0));
	}
}
