using UnityEngine;

public class GameOverCredits : MonoBehaviour
{
	[SerializeField]
	private GameObject _creditsPanel;

	[SerializeField]
	private float _scrollSpeed = 12.0f;

	private float _scrollStep = 0.0f;
	// Start is called before the first frame update
	private void Start()
	{
		_creditsPanel.transform.position = StartPosition();

		//	Calculate speed at which credits should (screen size / number of seconds * speed per frame)
		Debug.Log($"Smooth Delta Time: {Time.smoothDeltaTime}");
		_scrollStep = (Screen.height / _scrollSpeed) * Time.smoothDeltaTime;
	}

	// Update is called once per frame
	private void Update()
	{
		var rectTransform = _creditsPanel.transform as RectTransform;
		if ((rectTransform.position.y - rectTransform.rect.height / 2) < Screen.height)
			_creditsPanel.transform.position += new Vector3(0, _scrollStep, 0);
		else
			_creditsPanel.transform.position = StartPosition();
	}

	private Vector3 StartPosition()
	{
		var rectTransform = _creditsPanel.transform as RectTransform;
		return new Vector3(_creditsPanel.transform.position.x, 0 - rectTransform.rect.height / 2, transform.position.z);
	}
}
