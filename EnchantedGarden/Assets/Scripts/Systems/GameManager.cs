using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonBase<GameManager>
{
	//	TODO: Move these from here, I don't "think" we want to do it this way.
	//	Build a UI with some Events on
	//[SerializeField]
	//private Text _scoreText;

	//[SerializeField]
	//private Text _timeText;

	//[SerializeField]
	//private Text _gameOverText;

	[SerializeField]
	private ScriptableLevelDefinition _level;
	public ScriptableLevelDefinition Level { get { return _level; } }

	[SerializeField]
	private GameObject _gameOver;

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
		//_gameOver.SetActive(false);
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
		//	TODO: Put in overlay scene
		//_gameOverText.text = $"Game Over.\nYou scored {_score} points!";
		//_gameOver.SetActive(true);
	}
}
