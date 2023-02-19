using UnityEngine;
using UnityEngine.UI;

public class LevelTimerSlider : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	private void Start()
	{
		if (_slider == null)
			_slider = GetComponentInChildren<Slider>();
	}

	// Update is called once per frame
	private void Update()
	{
		_slider.value = GameManager.Instance.Elapsed / GameManager.Instance.ActiveLevel.LevelDuration;
	}
}
