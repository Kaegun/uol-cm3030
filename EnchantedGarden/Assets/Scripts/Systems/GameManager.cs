using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : SingletonBase<GameManager>
{
	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Levels")]
	//	TODO: Multiple levels
	[SerializeField]
	private ScriptableLevelDefinition[] _levels;
	public ScriptableLevelDefinition[] Levels => _levels;

	//	Track the currently active level
	private ScriptableLevelDefinition _level;
	public ScriptableLevelDefinition ActiveLevel => _level;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _backgroundMusicAudioSource;

	[SerializeField]
	private AudioSource _detachedAudioSourcePrefab;

	[SerializeField]
	private ScriptableAudioClip _gameOverMusic;

	public float Elapsed => _elapsedTime;

	private int _score;
	private float _elapsedTime = 0f;
	private bool _gameOver = false;

	//	TODO: Fix scoring - We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
	}

	public void RestartGame()
	{
		//	Restart time
		Time.timeScale = 1.0f;

		//	TODO: Restart game at current level?
		SceneLoader.LoadScene(CommonTypes.Scenes.Level1);
	}

	// Should probably be in the AudioController, would likely need to make it a Singleton though
	public AudioSource CreateDetachedAudioSource(Vector3 position)
	{
		return Instantiate(_detachedAudioSourcePrefab, position, Quaternion.identity);
	}

	//	TODO: Here we might be able to use an SO to raise events to all things that need to know about Game Ending, i.e. Sounds, etc.
	private void EndGame()
	{
		Debug.Log("Game Over");

		_gameOver = true;
		Time.timeScale = 0.0f;

		//	Play End of Game Audio loop
		AudioController.PlayAudio(_backgroundMusicAudioSource, _gameOverMusic);

		//	Load Game Over Screen
		SceneLoader.LoadScene(CommonTypes.Scenes.GameOver, true);
	}

	//	TODO: Level change logic
	private void SetCurrentActiveLevel()
	{
		_level = _levels[0];
		ActiveLevel.CurrentNumberOfPlants = ActiveLevel.StartNumberOfPlants;
	}

	//	Awake is called before Start
	protected override void Awake()
	{
		base.Awake();
		SetCurrentActiveLevel();
	}

	//	Start is called before the first frame update
	private void Start()
	{
		Assert.IsTrue(_levels.Length > 0);
		Assert.IsNotNull(_worldEvents);

		_worldEvents.PlantStolen += PlantStolen;

		_score = 0;
		SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
		AudioController.PlayAudio(_backgroundMusicAudioSource, _level.BackgroundMusic.lowIntensityAudio);
	}

	private void PlantStolen(object sender, Vector3 e)
	{
		ActiveLevel.CurrentNumberOfPlants--;
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
