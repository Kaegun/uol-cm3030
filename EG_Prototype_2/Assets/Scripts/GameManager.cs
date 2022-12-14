using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int _score;
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

    void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        _gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _timeRemaining -= Time.deltaTime;
        _timeText.text = $"Time: {_timeRemaining}";
        if (_timeRemaining <= 0 && !_gameOver.activeSelf)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Game Over");
        _gameOverText.text = $"Game Over.\nYou scored {_score} points!";
        _gameOver.SetActive(true);
    }

    public void ScorePoints(int points)
    {
        _score += points;
        _scoreText.text = $"Score: {_score}";
    }
}
