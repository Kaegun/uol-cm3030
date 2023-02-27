using UnityEngine;
using UnityEngine.Assertions;

public class ScoreIndicator : MonoBehaviour
{
	[SerializeField]
	private ScriptableWorldEventHandler _events;

	[SerializeField]
	private SpriteRenderer _scoreIndicator;

	private Camera _camera;

	public void SetIcon(Sprite scoreIndicator)
	{
		_scoreIndicator.sprite = scoreIndicator;
	}

	public bool Active => gameObject.activeSelf;

	public void SetActive(bool active) => gameObject.SetActive(active);

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_scoreIndicator, Utility.AssertNotNullMessage(nameof(_scoreIndicator)));
		Assert.IsNotNull(_events, Utility.AssertNotNullMessage(nameof(_events)));

		_camera = Camera.main;

		_events.Score += Score;
	}

	private void Score(object sender, ScriptableWorldEventHandler.ScoreEventArguments scoreEventArguments)
	{
		//	Display the score indicator over the player
		//	Do some animationy stuff
	}

	// Update is called once per frame
	private void Update()
	{
		transform.rotation = Quaternion.Euler(-_camera.transform.rotation.eulerAngles);
	}
}
