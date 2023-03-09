using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private GameObject _player;

	[SerializeField]
	private float _speed;

	// Start is called before the first frame update
	private void Start()
	{

	}

	/// <summary>
	/// Fixed Update is called on a specific interval for Physics.
	/// TODO: Should the camera movement be here or in Update?
	/// </summary>
	private void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, _player.transform.position, Time.deltaTime * _speed);
	}
}
