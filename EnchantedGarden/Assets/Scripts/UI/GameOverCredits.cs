using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameOverCredits : MonoBehaviour
{
	[SerializeField]
	private GameObject _creditsPanel;

	[SerializeField]
	private TMP_Text _buttonText;

	[SerializeField]
	private float _scrollSpeed = 12.0f;

	private float _scrollStep = 0.0f;

	private bool _scrolling = true;

	public void OnClick_PlayAgainContinue()
	{
		if (_buttonText.text == "Continue")
			GameManager.Instance.ContinueGame(CommonTypes.Scenes.Credits);
		else
			GameManager.Instance.RestartGame();
	}

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_creditsPanel, Utility.AssertNotNullMessage(nameof(_creditsPanel)));
		Assert.IsNotNull(_buttonText, Utility.AssertNotNullMessage(nameof(_buttonText)));

		//	TODO: Setting scrolling to false always. Not sure we still want this.
		_scrolling = false;

		//	A bit hacky
		if (SceneManager.GetActiveScene().name == CommonTypes.Scenes.Launcher)
		{
			_buttonText.text = "Continue";
		}

		_creditsPanel.transform.position = StartPosition();

		//	Calculate speed at which credits should (screen size / number of seconds * speed per frame)
		Debug.Log($"Smooth Delta Time: {Time.smoothDeltaTime}");
		_scrollStep = (Screen.height / _scrollSpeed) * Time.smoothDeltaTime;
	}

	// Update is called once per frame
	private void Update()
	{
		if (_scrolling)
		{
			var rectTransform = _creditsPanel.transform as RectTransform;
			if ((rectTransform.position.y - rectTransform.rect.height / 2) < Screen.height)
				_creditsPanel.transform.position += new Vector3(0, _scrollStep, 0);
			else
				_creditsPanel.transform.position = StartPosition();
		}
	}

	private Vector3 StartPosition()
	{
		if (_scrolling)
		{
			var rectTransform = _creditsPanel.transform as RectTransform;
			return new Vector3(_creditsPanel.transform.position.x, 0 - rectTransform.rect.height / 2, transform.position.z);
		}
		else
			return _creditsPanel.transform.position;
	}
}
