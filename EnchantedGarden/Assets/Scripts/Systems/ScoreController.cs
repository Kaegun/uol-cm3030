using UnityEngine;

public class ScoreController
{
	private ScriptableWorldEventHandler _events;
	private float _score = 0;
	private int _rating;

	public float Score => _score;

	public int Rating => _rating;

	public ScoreController(ScriptableWorldEventHandler events)
	{
		Debug.Log("Score Controller Loading");

		_events = events;

		_events.SpiritBanished += SpiritBanished;
		_events.SpiritWallBanished += SpiritWallBanished;

		Debug.Log("Score Controller Loaded");
	}

	public int CalculateRating(int numPlantsRemaining, int maxPlants)
	{
		//	TODO: Calculate final score and rating
		var plantRatio = numPlantsRemaining / (float)maxPlants;
		_score *= plantRatio > 0.8f ? 3 : plantRatio > 0.5f ? 2 : 1;

		return _rating = _score > 10000 ? 3 : _score > 5000 ? 2 : 1;
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
