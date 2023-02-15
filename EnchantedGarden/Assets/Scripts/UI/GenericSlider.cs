using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GenericSlider : MonoBehaviour, ISettable<float>
{
	[SerializeField]
	private float _maximumValue;

	[SerializeField]
	private bool _showText = true;

	[Header("Objects")]
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private TMPro.TMP_Text _text;

	public void SetMaximum(float value)
	{
		_maximumValue = value;
	}

	public void SetValue(float value)
	{
		_slider.value = value / _maximumValue;
		if (_showText)
			_text.text = $"{value} / {_maximumValue}";
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (_slider == null)
			_slider = GetComponentInChildren<Slider>();

		if (_text == null)
			_text = GetComponentInChildren<TMPro.TMP_Text>();

		Assert.IsNotNull(_slider);
		Assert.IsNotNull(_text);

		if (!_showText)
			_text.text = string.Empty;
	}
}
