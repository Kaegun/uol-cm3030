using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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

	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _sceneMusic;

	[SerializeField]
	private AudioSource _backgroundMusicAudioSource;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_backgroundMusicAudioSource, Utility.AssertNotNullMessage(nameof(_backgroundMusicAudioSource)));
		Assert.IsNotNull(_sceneMusic, Utility.AssertNotNullMessage(nameof(_sceneMusic)));

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
