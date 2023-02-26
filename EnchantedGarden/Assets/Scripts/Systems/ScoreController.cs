public class ScoreController
{
	private ScriptableWorldEventHandler _events;
	private float _score = 0;

	public float Score => _score;

	public ScoreController(ScriptableWorldEventHandler events)
	{
		_events = events;

		_events.SpiritBanished += SpiritBanished;
		_events.SpiritWallBanished += SpiritWallBanished;
	}

	public int CalculateRating(int numPlantsRemaining, int maxPlants)
	{
		//	TODO: Calculate final score and rating
		var plantRatio = numPlantsRemaining / (float)maxPlants;
		_score *= plantRatio > 0.8f ? 3 : plantRatio > 0.5f ? 2 : 1;

		return _score > 10000 ? 3 : _score > 5000 ? 2 : 1;
	}

	private void SpiritBanished(object sender, Spirit e)
	{
		_score += CommonTypes.ScoreValues.BanishedSpirit;
		_events.OnScore(CommonTypes.ScoreValues.BanishedSpirit);
	}

	private void SpiritWallBanished(object sender, SpiritWall e)
	{
		_score += CommonTypes.ScoreValues.BanishedWall;
		_events.OnScore(CommonTypes.ScoreValues.BanishedWall);
	}
}
