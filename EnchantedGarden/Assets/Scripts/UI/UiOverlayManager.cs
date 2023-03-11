using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

//	General UI handling code
public class UiOverlayManager : MonoBehaviour
{
	public enum OverlayElements
	{
		Fire,
		Cauldron,
		Time,
		Score,
		Plants
	}

	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _events;

	[Header("UI Controls")]
	[SerializeField]
	private GameObject _fireSlider;

	[SerializeField]
	private GameObject _plantSlider;

	[SerializeField]
	private GameObject _usesSlider;

	[SerializeField]
	private GameObject _score;

	[SerializeField]
	private GameObject _timeSlider;

	[Header("Scoring")]
	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private FloatingScoreIndicator _floatingScorePrefab;

	[SerializeField]
	private float _scoreTextduration;

	private ISettable<float> _fireSliderSettable,
		_plantSliderSettable,
		_usesSliderSettable,
		_scoreSettable;

	private void SubscribeToEvents()
	{
		_events.Score += Score;
	}

	private void UnsubscribeFromEvents()
	{
		_events.Score -= Score;
	}

	private void Start()
	{
		Assert.IsNotNull(_events, Utility.AssertNotNullMessage(nameof(_events)));
		Assert.IsNotNull(_fireSlider, Utility.AssertNotNullMessage(nameof(_fireSlider)));
		Assert.IsNotNull(_plantSlider, Utility.AssertNotNullMessage(nameof(_plantSlider)));
		Assert.IsNotNull(_usesSlider, Utility.AssertNotNullMessage(nameof(_usesSlider)));
		Assert.IsNotNull(_score, Utility.AssertNotNullMessage(nameof(_score)));

		if (!_fireSlider.TryGetComponent(out _fireSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Fire Slider is not a valid ISettable"));

		if (!_plantSlider.TryGetComponent(out _plantSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Plant Slider is not a valid ISettable"));

		if (!_usesSlider.TryGetComponent(out _usesSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Uses Slider is not a valid ISettable"));

		if (!_score.TryGetComponent(out _scoreSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Score control is not a valid ISettable"));

		_fireSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.CauldronSettings.FireDuration);
		_plantSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.StartNumberOfPlants);
		_usesSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.CauldronSettings.MaximumUses);

		foreach (var overlayElement in GameManager.Instance.DisabledOverlayElements)
		{
			switch (overlayElement)
			{
				case OverlayElements.Fire:
					_fireSlider.SetActive(false);
					break;
				case OverlayElements.Cauldron:
					_usesSlider.SetActive(false);
					break;
				case OverlayElements.Time:
					_timeSlider.SetActive(false);
					break;
				case OverlayElements.Score:
					_score.SetActive(false);
					break;
				case OverlayElements.Plants:
					_plantSlider.SetActive(false);
					break;
				default:
					break;
			}
		}

		_scoreSettable.SetValue(0);

		SubscribeToEvents();
	}

	private void Score(object sender, ScriptableWorldEventHandler.ScoreEventArguments e)
	{
		StartCoroutine(ScoreCoroutine(e));
	}

	private IEnumerator ScoreCoroutine(ScriptableWorldEventHandler.ScoreEventArguments e)
	{
		//	Only show score animation when score is not zerossss
		if (e.Score > 0)
		{
			var floatingScore = Instantiate(_floatingScorePrefab, Camera.main.WorldToScreenPoint(e.Position), Quaternion.identity, _canvas.transform);
			floatingScore.SetProperties(e.Score);
			var startPos = floatingScore.transform.position;
			float t = 0f;
			while (t <= _scoreTextduration)
			{
				floatingScore.transform.position = Vector3.Lerp(startPos, _scoreSettable.Transform.position, t / _scoreTextduration);
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			Destroy(floatingScore.gameObject);
		}
		_scoreSettable.SetValue(e.Score);
	}

	private void Update()
	{
		_usesSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses);
		_plantSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CurrentNumberOfPlants);
		_fireSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentFireLevel);
	}

	private void OnDestroy()
	{
		UnsubscribeFromEvents();
	}
}
