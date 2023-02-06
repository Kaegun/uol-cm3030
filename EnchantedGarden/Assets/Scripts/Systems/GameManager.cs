using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonBase<GameManager>
{
	//	TODO: Move these from here, I don't "think" we want to do it this way.
	//	Build a UI with some Events on
	[SerializeField]
	private Text _scoreText;

	[SerializeField]
	private Text _timeText;

	[SerializeField]
	private Text _gameOverText;

	[SerializeField]
	private ScriptableLevelDefinition _level;

	[SerializeField]
	private GameObject _gameOver;

	private int _score;
	private float _timeRemaining;

	//	We can use a SO for this
	public void ScorePoints(int points)
	{
		_score += points;
		_scoreText.text = $"Score: {_score}";
	}

	//	Start is called before the first frame update
	private void Start()
	{
		_score = 0;
		_timeRemaining = _level.LevelDuration;
		_gameOver.SetActive(false);
	}

	//	Update is called once per frame
	private void Update()
	{
		//	TODO: Let's rather count upwards towards the remaining time, and put it in the level definition
		_timeRemaining -= Time.deltaTime;
		_timeText.text = $"Time: {_timeRemaining}";
		if (_timeRemaining <= 0 && !_gameOver.activeSelf)
		{
			EndGame();
		}
	}

	//	TODO: Here we might be able to use an SO to raise events to all things that need to know about Game Ending, i.e. Sounds, etc.
	private void EndGame()
	{
		Debug.Log("Game Over");
		_gameOverText.text = $"Game Over.\nYou scored {_score} points!";
		_gameOver.SetActive(true);
	}
}
