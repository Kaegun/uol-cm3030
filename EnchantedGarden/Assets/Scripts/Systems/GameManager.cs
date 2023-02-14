using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
	[SerializeField]
	private ScriptableLevelDefinition _level;
	public ScriptableLevelDefinition Level => _level;

	//	TODO: Multiple levels
	[SerializeField]
	private ScriptableLevelDefinition[] _levels;
	public ScriptableLevelDefinition[] Levels => _levels;

	public float Elapsed => _elapsedTime;

	private int _score;
	private float _elapsedTime = 0f;

	//	TODO: Fix scoring - We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
		//_scoreText.text = $"Score: {_score}";
	}

	public void RestartGame()
	{
		//	TODO: Restart game at current level?
		SceneLoader.LoadScene(CommonTypes.Scenes.Level1);
	}

	//	TODO: Here we might be able to use an SO to raise events to all things that need to know about Game Ending, i.e. Sounds, etc.
	private void EndGame()
	{
		Debug.Log("Game Over");

		Time.timeScale = 0.0f;

		//	Load Game Over Screen
		SceneLoader.LoadScene(CommonTypes.Scenes.GameOver, true);
	}

	//	Start is called before the first frame update
	private void Start()
	{
		_score = 0;

		//	TODO: Testing
		SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
	}

	//	Update is called once per frame
	private void Update()
	{
		_elapsedTime += Time.deltaTime;

		//	TODO: Update DayNightSlider with remaining time
		//_timeText.text = $"Time: {Level.LevelDuration - _elapsedTime}";
		if (_elapsedTime >= Level.LevelDuration /*&& !_gameOver.activeSelf*/)
		{
			EndGame();
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
