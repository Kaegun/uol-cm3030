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
		Debug.Log($"SCore Coroutine: {score} | {GameManager.Instance.Score.Score}");
		_text.text = $"SCORE: {GameManager.Instance.Score.Score:# ##0}";
		yield return new WaitForEndOfFrame();
	}
}

