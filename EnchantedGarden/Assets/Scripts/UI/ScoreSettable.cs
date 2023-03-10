using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreSettable : MonoBehaviour, ISettable<float>
{
	[SerializeField]
	private TMP_Text _text;

	private Coroutine _scoreUpdating;

	public Transform Transform => transform;

	public void SetMaximum(float value)
	{
		throw new NotImplementedException();
	}

	public void SetValue(float value)
	{
		_scoreUpdating = StartCoroutine(ScoreUpdatingCoroutine(value));
	}

	private IEnumerator ScoreUpdatingCoroutine(float score)
	{
		_text.text = $"SCORE: {GameManager.Instance.Score.Score:# ##0}";
		//	TODO: Animate the score text in some way to make it pop when the player scores points.

		float animationDuration = 0.4f;
		float growAmount = 1.2f;
		// Grow
		float t = 0f;
		while (t < animationDuration / 2f)
        {
			_text.transform.localScale = Vector3.Lerp(Vector3.one, growAmount * Vector3.one, t / (animationDuration / 2f));
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		// Shrink
		t = 0f;
		while (t < animationDuration / 2f)
		{
			_text.transform.localScale = Vector3.Lerp(growAmount * Vector3.one, Vector3.one, t / (animationDuration / 2f));
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		_text.transform.localScale = Vector3.one;
		yield return null;
	}

	private void Update()
	{
		
	}
}

