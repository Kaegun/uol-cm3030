﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

//	General UI handling code
public class UiOverlayManager : MonoBehaviour
{
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

		_events.Score += Score;
		_scoreSettable.SetValue(0);
	}

	private void Score(object sender, ScriptableWorldEventHandler.ScoreEventArguments e)
	{
		Debug.Log($"Scored Points: {e.Score}");
		StartCoroutine(ScoreCoroutine(e));
	}

	private IEnumerator ScoreCoroutine(ScriptableWorldEventHandler.ScoreEventArguments e)
    {		
		var floatingScore = Instantiate(_floatingScorePrefab, Camera.main.WorldToScreenPoint(e.Position), Quaternion.identity, _canvas.transform);
		floatingScore.SetProperties(e.Score);
		var startPos = floatingScore.transform.position;
		float t = 0f;
		while (t <= _scoreTextduration)
        {
			//	TODO: Make score text movement better
			floatingScore.transform.position = Vector3.Lerp(startPos, _scoreSettable.Transform.position, t / _scoreTextduration);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
        }
		Destroy(floatingScore.gameObject);
		_scoreSettable.SetValue(e.Score);
    }

	private void Update()
	{
		_usesSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses);
		_plantSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CurrentNumberOfPlants);
		_fireSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentFireLevel);
	}
}
