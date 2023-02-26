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
		_events.PlantReplanted += PlantReplanted;

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
		int scoreValue = 0;
		if (e.ActiveSpiritState == Spirit.SpiritState.StartingPossession)
        {
			scoreValue = CommonTypes.ScoreValues.BanishedSpiritBeforeFinishedPossession;
		}
		else if (e.ActiveSpiritState == Spirit.SpiritState.Possessing)
        {
			scoreValue = CommonTypes.ScoreValues.BanishedSpiritAfterFinishedPossession;
		}
		_score += scoreValue;
		_events.OnScore(new ScriptableWorldEventHandler.ScoreEventArguments(scoreValue, e.transform.position));
	}

	private void SpiritWallBanished(object sender, SpiritWall e)
	{
		_score += CommonTypes.ScoreValues.BanishedWall;
		_events.OnScore(new ScriptableWorldEventHandler.ScoreEventArguments(CommonTypes.ScoreValues.BanishedWall, e.transform.position));
	}

	private void PlantReplanted(object sender, Vector3 e)
    {
		_score += CommonTypes.ScoreValues.ReplantedPlant;
		_events.OnScore(new ScriptableWorldEventHandler.ScoreEventArguments(CommonTypes.ScoreValues.ReplantedPlant, e));
    }
}
