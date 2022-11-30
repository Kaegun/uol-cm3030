using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int _score = 0;

    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _timerText;

    [SerializeField]
    private GameObject _gameOverUI;
    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private float _timeLimit;

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

    void Start()
    {
        _scoreText.text = "Score: " + _score;
    }

    void Update()
    {
        _timeLimit -= Time.deltaTime;
        _timerText.text = "Time: " + _timeLimit;
        if (_timeLimit <= 0.0f && _gameOverUI.activeSelf == false)
        {
            EndGame();
        }
    }

    public void ScorePoints(int points)
    {
        _score += points;
        _scoreText.text = "Score: " + _score;
    }

    public void EndGame()
    {
        // disable spirit manager to stop spawning spirits
        SpiritManager.instance.gameObject.SetActive(false);

        // activate game over screen
        _gameOverText.text = "Game Over \nYou scored " + _score + " points.";
        _gameOverUI.SetActive(true);
    }
}
