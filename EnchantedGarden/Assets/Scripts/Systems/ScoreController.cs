using UnityEngine;

public class ScoreController
{
	private ScriptableWorldEventHandler _events;
	private float _score = 0;
	private int _finalScore = 0;
	private int _rating;

	public float Score => _score;
	public int FinalScore => _finalScore;

	public int Rating => _rating;

	public ScoreController(ScriptableWorldEventHandler events)
	{
		Debug.Log("Score Controller Loading");

		_events = events;

		_events.SpiritBanished += SpiritBanished;
		_events.SpiritWallBanished += SpiritWallBanished;

		Debug.Log("Score Controller Loaded");
	}

	private void CalculateFinalScore(int numPlantsRemaining)
    {
		// Final score is score +50% for each plant remaining
		_finalScore = (int)(_score * (1 + 0.5f * numPlantsRemaining));
	}

	public int CalculateRating(int numPlantsRemaining, int twoStarThreshold, int threeStarThreshold)
	{
		CalculateFinalScore(numPlantsRemaining);
		return _rating = _finalScore >= threeStarThreshold ? 3 : _finalScore >= twoStarThreshold ? 2 : 1;
	}

	private void SpiritBanished(object sender, Spirit e)
	{
		Debug.Log("ScoreController.SpiritBanished");
		_score += CommonTypes.ScoreValues.BanishedSpirit;
		_events.OnScore(new ScriptableWorldEventHandler.ScoreEventArguments(CommonTypes.ScoreValues.BanishedSpirit, e.transform.position));
	}

	private void SpiritWallBanished(object sender, SpiritWall e)
	{
		Debug.Log("ScoreController.SpiritWallBanished");
		_score += CommonTypes.ScoreValues.BanishedWall;
		_events.OnScore(new ScriptableWorldEventHandler.ScoreEventArguments(CommonTypes.ScoreValues.BanishedWall, e.transform.position));
	}
}
