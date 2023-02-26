using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreSettable : MonoBehaviour, ISettable<float>
{
	[SerializeField]
	private TMP_Text _text;

	private Coroutine _scoreUpdating;
	public void SetMaximum(float value)
	{
		throw new NotImplementedException();
	}

	public void SetValue(float value)
	{
		Debug.Log("Setting the score");
		_scoreUpdating = StartCoroutine("ScoreUpdatingCoroutine", value);
	}

	private IEnumerable ScoreUpdatingCoroutine(float score)
	{
		Debug.Log($"Score Coroutine: {score} | {GameManager.Instance.Score.Score}");

		//	TODO: Animate the score text in some way to make it pop when the player scores points.

		yield return new WaitForEndOfFrame();
	}

	private void Update()
	{
		_text.text = $"SCORE: {GameManager.Instance.Score.Score:# ##0}";
	}
}

