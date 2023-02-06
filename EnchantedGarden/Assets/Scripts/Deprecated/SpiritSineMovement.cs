using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiritSineMovement : MonoBehaviour
{
	[SerializeField]
	private float _acceleration = 10.0f;

	[SerializeField]
	private float _maxSpeed = 5.0f;

	[SerializeField]
	private float _turnSpeed = 90.0f;

	[SerializeField]
	private float _amplitude = 0.5f;

	[SerializeField]
	private float _frequency = 1.0f;

	private float _angle = 0.0f,
					_angleOffset = 0.0f;
	private Rigidbody _rb;

	// Start is called before the first frame update
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Awake()
	{
		//  Check where the spirit is, and choose direction to move in
		//  For now, move towards the center

		var relativePosition = Vector3.zero - transform.position;
		_angleOffset = Vector3.Angle(Vector3.up, relativePosition);
		Debug.Log($"Relative Pos: {relativePosition} | {_angleOffset}");

		//	For now, snap to look at the center
		transform.LookAt(Vector3.zero);

		//	Below to Slerp the rotation
		//Quaternion rotation = Quaternion.LookRotation(relativePosition);
		//Quaternion current = transform.localRotation;
		//transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * speed);
	}

	// Update is called once per frame
	private void Update()
	{
		_angle += Time.deltaTime;
		var x = Mathf.Cos(_angle + _angleOffset) * _frequency;
		var y = Mathf.Sin(_angle + _angleOffset) * _amplitude;

		Debug.Log($"x: {x} | y: {y}");

		//	If we're at the center, stop
		if ((transform.position - Vector3.zero).magnitude > 0.001f)
		{
			var direction = Quaternion.Euler(0, Vector2.SignedAngle(Vector2.up, new Vector2(x, y)), 0);
			var turnDirection = Quaternion.RotateTowards(transform.rotation, direction, _turnSpeed * Time.deltaTime);

			Debug.Log($"Sine Wave Attempt: {direction} | {turnDirection} | {Vector2.SignedAngle(Vector2.up, new Vector2(x, y))}");

			_rb.MoveRotation(turnDirection.normalized);
			_rb.AddForce(_acceleration * Time.deltaTime * transform.forward, ForceMode.Impulse);
		}
		else
		{
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;
		}
	}
}
