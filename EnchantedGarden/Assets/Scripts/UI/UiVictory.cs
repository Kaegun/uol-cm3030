using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiVictory : MonoBehaviour
{
	[Header("Stars")]
	[SerializeField]
	private Image[] _stars = new Image[3];

	[SerializeField]
	private Sprite _starFilled;

	[SerializeField]
	private Sprite _starEmpty;

	[SerializeField]
	private TMP_Text _scoreText;

	// Start is called before the first frame update
	void Start()
	{
		//  Get Score from Game Manager
		var score = GameManager.Instance.Score;

		//  Set Stars
		for (int i = 0; i < 3; i++)
		{
			_stars[i].sprite = score.Rating > i ? _starFilled : _starEmpty;
		}

		//  Set Score
		_scoreText.text = $"{score.FinalScore:# ##0}";
	}
}
