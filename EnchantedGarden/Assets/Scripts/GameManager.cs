using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;
	public static GameManager Instance { get; }

	[SerializeField]
	private Text _scoreText;

	[SerializeField]
	private float _timeRemaining;

	[SerializeField]
	private Text _timeText;

	[SerializeField]
	private GameObject _gameOver;

	[SerializeField]
	private Text _gameOverText;

	private int _score;

	public void ScorePoints(int points)
	{
		_score += points;
		_scoreText.text = $"Score: {_score}";
	}

	private void Awake()
	{
		if (_instance == null && _instance != this)
		{
			_instance = this;
		}
		else
		{
			//	This shouldn't be possible
			Destroy(this);
		}
	}

	//	Start is called before the first frame update
	private void Start()
	{
		_score = 0;
		_gameOver.SetActive(false);
	}

	//	Update is called once per frame
	private void Update()
	{
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
