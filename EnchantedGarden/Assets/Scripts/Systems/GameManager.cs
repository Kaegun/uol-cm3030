using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	[SerializeField]
	private ScriptableLevelDefinition _level;
	public ScriptableLevelDefinition Level => _level;

	[SerializeField]
	private AudioSource _backgroundMusicAudioSource;

	public float Elapsed => _elapsedTime;

	private int _score;
	private float _elapsedTime = 0f;

	//	We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
		//_scoreText.text = $"Score: {_score}";
	}

	//	Start is called before the first frame update
	private void Start()
	{
		_score = 0;
		AudioController.PlayAudio(_backgroundMusicAudioSource, _level.BackgroundMusic.lowIntensityAudio);
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

	//	TODO: Here we might be able to use an SO to raise events to all things that need to know about Game Ending, i.e. Sounds, etc.
	private void EndGame()
	{
		Debug.Log("Game Over");

		Time.timeScale = 0.0f;

		//	Load Game Over Screen
		SceneManager.LoadSceneAsync(CommonTypes.Scenes.GameOver, LoadSceneMode.Additive);
	}
}
