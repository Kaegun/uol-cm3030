using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

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

	[Header("UI")]
	[SerializeField]
	private bool _useUiOverlay = true;

	private int _score;
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

	public void ContinueGame()
	{
		//	Restart time
		Time.timeScale = 1.0f;

		//	TODO: Track scene that caused the pause?
		SceneLoader.UnloadScene(CommonTypes.Scenes.Options);
	}

	// Should probably be in the AudioController, would likely need to make it a Singleton though
	public AudioSource CreateDetachedAudioSource(Vector3 position)
	{
		return Instantiate(_detachedAudioSourcePrefab, position, Quaternion.identity);
	}

	public void CheckIngredientsLow()
	{
		if ((float)ActiveLevel.CauldronSettings.CurrentNumberOfUses / ActiveLevel.CauldronSettings.MaximumUses < CommonTypes.Constants.UsesThreshold)
			_worldEvents.OnIngredientsLowWarning(transform.position);
	}

	public void CheckIngredientsEmpty()
    {
		if (ActiveLevel.CauldronSettings.CurrentNumberOfUses == 0)
			_worldEvents.OnIngredientsEmpty(transform.position);
	}

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
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		//	Testing
		var numPlants = 0;
		foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			numPlants += go.GetComponentsInChildren<Plant>().Length;
		}

		Debug.Log($"Dynamic number of plants [{numPlants}]");

		_worldEvents.PlantStolen += PlantStolen;

		_score = 0;
		if (_useUiOverlay)
		{
			SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
		}
		AudioController.PlayAudio(_backgroundMusicAudioSource, _level.BackgroundMusic.lowIntensityAudio);
		_worldEvents.OnLevelStarted(CommonTypes.Scenes.Level0);
	}

	private void PlantStolen(object sender, GameObject e)
	{
		ActiveLevel.CurrentNumberOfPlants -= 1;
		if (ActiveLevel.CurrentNumberOfPlants <= 0)
			EndGame();
	}

	//	Update is called once per frame
	private void Update()
	{
		if (Time.timeSinceLevelLoad >= ActiveLevel.LevelDuration && !_gameOver)
		{
			EndGame();
		}
	}
}
