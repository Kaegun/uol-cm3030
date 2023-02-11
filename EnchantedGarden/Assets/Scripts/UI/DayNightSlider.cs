using UnityEngine;
using UnityEngine.UI;

public class DayNightSlider : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private float _cycleTime = 30;
	public float CycleTime
	{
		get { return _cycleTime; }
		set { _cycleTime = value; }
	}

	private void Start()
	{
		if (_slider == null)
			_slider = GetComponentInChildren<Slider>();
	}

	// Update is called once per frame
	private void Update()
	{		
		_slider.value = GameManager.Instance.Elapsed / GameManager.Instance.Level.LevelDuration;
	}
}
