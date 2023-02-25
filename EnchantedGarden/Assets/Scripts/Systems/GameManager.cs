﻿using System;
using System.Collections;
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

	private int _score;
	private bool _gameOver = false;
	private int _activeSpiritCount = 0;

	//	TODO: Fix scoring - We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
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

	public void RestartGame()
	{
		//	Restart time
		//Time.timeScale = 1.0f;
		SceneLoader.LoadScene(_activeLevel.Level.SceneName());
	}

	public void ContinueGame()
	{
		//	Restart time
		Time.timeScale = 1.0f;

		//	TODO: Track scene that caused the pause?
		SceneLoader.UnloadScene(CommonTypes.Scenes.Options);
	}

	public void LoadNextLevel()
    {
		int currentLevelIndex = Array.IndexOf(_gameLevels, _activeLevel);
		if (currentLevelIndex < _gameLevels.Length - 1)
        {
			var nextLevel = _gameLevels[currentLevelIndex + 1];
			SceneLoader.LoadScene(nextLevel.Level.SceneName());
		}
		else
        {
			// Load credits scene
			//SceneLoader.LoadScene(CommonTypes.Scenes.Credits);
		}
    }	

	private void LevelFailed()
	{
		Debug.Log("Game Over");

		_gameOver = true;
		Time.timeScale = 0.0f;

		//	Play End of Game Audio loop
		AudioController.PlayAudio(_backgroundMusicAudioSource, _gameOverMusic);

		//	Load Game Over Screen
		SceneLoader.LoadScene(CommonTypes.Scenes.GameOver, true);
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
		SceneLoader.LoadScene(CommonTypes.Scenes.Victory, true);
	}

	//	TODO: Level change logic
	private void SetCurrentActiveLevel()
	{
		//_level = _levels[0];
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
		Assert.IsTrue(_gameLevels.Length > 0);
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		//	Testing
		var numPlants = 0;
		foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			numPlants += go.GetComponentsInChildren<Plant>().Length;
		}

		Debug.Log($"Dynamic number of plants [{numPlants}]");

		_worldEvents.PlantStolen += PlantStolen;
		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritBanished += SpiritBanished;

		_score = 0;
		if (_useUiOverlay)
		{
			SceneLoader.LoadScene(CommonTypes.Scenes.UI, true);
		}
		AudioController.PlayAudio(_backgroundMusicAudioSource, _activeLevel.BackgroundMusic.lowIntensityAudio);
		StartCoroutine(LevelStartedEventCoroutine(CommonTypes.Scenes.Level0));
		Time.timeScale = 1.0f;
	}

	private void PlantStolen(object sender, GameObject e)
	{
		ActiveLevel.CurrentNumberOfPlants -= 1;
		if (ActiveLevel.CurrentNumberOfPlants <= 0)
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

	}

	//	Update is called once per frame
	private void Update()
	{
		if (Time.timeSinceLevelLoad >= ActiveLevel.LevelDuration && !_gameOver)
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
}
