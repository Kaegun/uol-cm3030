using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TestSunSlider : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	private float _direction = 1.0f;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		_slider.value += Time.deltaTime * _direction;
		if (_slider.value >= 1.0f)
			_direction = -1.0f;
		else if (_slider.value <= 0.0f)
			_direction = 1.0f;
	}
}
