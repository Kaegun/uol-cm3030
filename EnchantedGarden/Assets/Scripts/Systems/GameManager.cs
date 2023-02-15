using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
	//	TODO: Multiple levels
	[SerializeField]
	private ScriptableLevelDefinition[] _levels;
	public ScriptableLevelDefinition[] Levels => _levels;

	//	Track the currently active level
	private ScriptableLevelDefinition _level;
	public ScriptableLevelDefinition ActiveLevel => _level;

	[SerializeField]
	private AudioSource _backgroundMusicAudioSource;

	public float Elapsed => _elapsedTime;

	public int NumberOfPlants => _numberOfPlants;

	private int _score;
	private float _elapsedTime = 0f;
	private bool _gameOver = false;
	private int _numberOfPlants = 5;

	//	TODO: Fix scoring - We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
		//_scoreText.text = $"Score: {_score}";
	}

	public void RestartGame()
	{
		//	TODO: Restart game at current level?

		Time.timeScale = 0.0f;

		SceneLoader.LoadScene(CommonTypes.Scenes.Level1);
	}

	//	TODO: Here we might be able to use an SO to raise events to all things that need to know about Game Ending, i.e. Sounds, etc.
	private void EndGame()
	{
		Debug.Log("Game Over");

		_gameOver = true;
		Time.timeScale = 0.0f;

		//	Load Game Over Screen
		SceneLoader.LoadScene(CommonTypes.Scenes.GameOver, true);
	}

	//	Start is called before the first frame update
	private void Start()
	{
		_score = 0;
		_level = _levels[0];
		SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
		AudioController.PlayAudio(_backgroundMusicAudioSource, _level.BackgroundMusic.lowIntensityAudio);
	}

	//	Update is called once per frame
	private void Update()
	{
		_elapsedTime += Time.deltaTime;

		if (_elapsedTime >= ActiveLevel.LevelDuration && !_gameOver)
		{
			EndGame();
		}
	}
}
