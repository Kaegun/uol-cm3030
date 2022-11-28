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

    public void ScorePoints(int points)
    {
        _score += points;
        _scoreText.text = "Score: " + _score;
    }
}
