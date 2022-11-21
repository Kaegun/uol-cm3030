using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float _maxSpeed = 2.0f;

	[SerializeField]
	private float _turnSpeed = 10.0f;

	private Vector2 _moveDirection = Vector2.zero;
	//private float _currentSpeed = 0.0f;

	//	Reference: Penny De Byl - 3rd Person course (Udemy)
	private bool IsMoving => !Mathf.Approximately(_moveDirection.sqrMagnitude, 0.0f);

	public void OnMove(InputAction.CallbackContext context)
	{
		//  Only act on completed movement key events
		if (context.performed || context.canceled)
		{
			_moveDirection = context.ReadValue<Vector2>();
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//transform.SetPositionAndRotation(_maxSpeed * Time.deltaTime * new Vector3(0, 0, _moveDirection.y),
		//	Quaternion.LookRotation(_turnSpeed * Time.deltaTime * new Vector3(0, 0, _moveDirection.x), transform.up));

		transform.Translate(_maxSpeed * Time.deltaTime * new Vector3(0, 0, _moveDirection.y));
		transform.Rotate(_turnSpeed * Time.deltaTime * new Vector3(0, _moveDirection.x, 0));
	}
}
