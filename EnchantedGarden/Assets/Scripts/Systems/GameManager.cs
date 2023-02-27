using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : SingletonBase<GameManager>
{
	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Levels")]
	[SerializeField]
	private ScriptableLevelDefinition[] _gameLevels;
	public ScriptableLevelDefinition[] GameLevels => _gameLevels;

	//	Track the currently active level
	[SerializeField]
	private ScriptableLevelDefinition _activeLevel;
	public ScriptableLevelDefinition ActiveLevel => _activeLevel;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _backgroundMusicAudioSource;

	[SerializeField]
	private AudioSource _detachedAudioSourcePrefab;

	[SerializeField]
	private ScriptableAudioClip _gameOverMusic;

	[SerializeField]
	private int _midIntensityMusicThreshold;

	[SerializeField]
	private int _highIntensityMusicThreshold;

	[Header("UI")]
	[SerializeField]
	private bool _useUiOverlay = true;

	[SerializeField]
	private UiOverlayManager.OverlayElements[] _disabledOverlayElements;

	public UiOverlayManager.OverlayElements[] DisabledOverlayElements => _disabledOverlayElements;

	private bool _gameOver = false;
	private int _activeSpiritCount = 0;

	public ScoreController Score { get; private set; }

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

	public void ReturnToLauncher()
	{
		SceneLoader.LoadScene(CommonTypes.Scenes.Launcher);
	}

	public void RestartGame()
	{
		SceneLoader.LoadScene(CommonTypes.Scenes.Level0);
	}

	public void RestartLevel()
	{
		//	Restart time
		//Time.timeScale = 1.0f;
		SceneLoader.LoadScene(_activeLevel.Level.SceneName());
	}

	public void ContinueGame(string sceneName)
	{
		//	Restart time
		Time.timeScale = 1.0f;

		SceneLoader.UnloadScene(sceneName);
	}

	public void LoadNextLevel()
	{
		int currentLevelIndex = Array.IndexOf(_gameLevels, _activeLevel);
		Debug.Log($"LeadNextLevel - CurrentLevelIndex: {currentLevelIndex}");
		if (currentLevelIndex < _gameLevels.Length - 1)
		{
			var nextLevel = _gameLevels[currentLevelIndex + 1];
			Debug.Log($"LeadNextLevel - CurrentLevelIndex: {nextLevel.Level.SceneName()}");
			SceneLoader.LoadScene(nextLevel.Level.SceneName());
		}
		else
		{
			// Load credits scene
			SceneLoader.UnloadScene(CommonTypes.Scenes.UI);
			SceneLoader.UnloadScene(CommonTypes.Scenes.Victory);
			SceneLoader.LoadScene(CommonTypes.Scenes.Credits, true);
		}
	}

	private void LevelFailed()
	{
		Debug.Log("Level Failed");

		Time.timeScale = 0.0f;

		//	Play End of Game Audio loop
		AudioController.PlayAudio(_backgroundMusicAudioSource, _gameOverMusic);

		//	Load Game Over Screen
		SceneLoader.LoadScene(CommonTypes.Scenes.LevelFailed, true);
	}

	private void EndLevel(bool victory)
	{
		Debug.Log("Level finished");

		_gameOver = true;
		Time.timeScale = 0.0f;

		if (victory)
		{
			LoadVictoryScene();
		}
		else
		{
			LevelFailed();
		}
	}

	private void LoadVictoryScene()
	{
		Score.CalculateRating(ActiveLevel.CurrentNumberOfPlants, ActiveLevel.TwoStarScoreThreshold, ActiveLevel.ThreeStarScoreThreshold);
		SceneLoader.LoadScene(CommonTypes.Scenes.Victory, true);
	}

	private void SetCurrentActiveLevel()
	{
		//	TODO: Check this?
		if (_activeLevel == null)
			_activeLevel = _gameLevels[0];
		ActiveLevel.CurrentNumberOfPlants = ActiveLevel.StartNumberOfPlants;
	}

	//	Awake is called before Start
	protected override void Awake()
	{
		base.Awake();
		SetCurrentActiveLevel();
	}

	private void PlantStolen(object sender, GameObject e)
	{
		Debug.Log($"GameManager.CurrentNumberOfPlants: {ActiveLevel.name}:{ActiveLevel.CurrentNumberOfPlants}");
		if (--ActiveLevel.CurrentNumberOfPlants <= 0)
			EndLevel(false);
	}

	private void SpiritSpawned(object sender, Spirit e)
	{
		_activeSpiritCount++;
	}

	private void SpiritBanished(object sender, Spirit e)
	{
		_activeSpiritCount--;
	}

	private IEnumerator LevelStartedEventCoroutine(string level)
	{
		yield return new WaitForSeconds(0.25f);
		_worldEvents.OnLevelStarted(level);
		CheckIngredientsEmpty();
	}

	private void SubscribeToWorldEvents()
	{
		_worldEvents.PlantStolen += PlantStolen;
		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritBanished += SpiritBanished;
	}

	private void UnsubscribeFromWorldEvents()
	{
		_worldEvents.PlantStolen -= PlantStolen;
		_worldEvents.SpiritSpawned -= SpiritSpawned;
		_worldEvents.SpiritBanished -= SpiritBanished;
	}

	//	Start is called before the first frame update
	private void Start()
	{
		Assert.IsTrue(_gameLevels.Length > 0);
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		SubscribeToWorldEvents();

		if (_useUiOverlay)
		{
			SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
		}

		AudioController.PlayAudio(_backgroundMusicAudioSource, _activeLevel.BackgroundMusic.lowIntensityAudio);
		StartCoroutine(LevelStartedEventCoroutine(CommonTypes.Scenes.Level0));
		Time.timeScale = 1.0f;

		Score = new ScoreController(_worldEvents);
	}

	//	Update is called once per frame
	private void Update()
	{
		if (!ActiveLevel.InfiniteLevel && Time.timeSinceLevelLoad >= ActiveLevel.LevelDuration && !_gameOver)
		{
			EndLevel(true);
		}

		if (!_gameOver && _backgroundMusicAudioSource.timeSamples > _backgroundMusicAudioSource.clip.samples * 0.999f)
		{
			if (_activeSpiritCount < _midIntensityMusicThreshold)
			{
				AudioController.PlayAudio(_backgroundMusicAudioSource, _activeLevel.BackgroundMusic.lowIntensityAudio);
			}
			else if (_activeSpiritCount >= _midIntensityMusicThreshold && _activeSpiritCount < _highIntensityMusicThreshold)
			{
				AudioController.PlayAudio(_backgroundMusicAudioSource, _activeLevel.BackgroundMusic.midIntensityAudio);
			}
			else if (_activeSpiritCount >= _highIntensityMusicThreshold)
			{
				AudioController.PlayAudio(_backgroundMusicAudioSource, _activeLevel.BackgroundMusic.highIntensityAudio);
			}
		}
	}

	private void OnDestroy()
	{
		UnsubscribeFromWorldEvents();
		Score = null;
	}
}
