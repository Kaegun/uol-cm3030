using UnityEngine;
using UnityEngine.Assertions;

//	General UI handling code
public class UiOverlayManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _fireSlider;

	[SerializeField]
	private GameObject _plantSlider;

	[SerializeField]
	private GameObject _usesSlider;

	private ISettable<float> _fireSliderSettable,
		_plantSliderSettable,
		_usesSliderSettable;

	private void Start()
	{
		Assert.IsNotNull(_fireSlider, Utility.AssertNotNullMessage(nameof(_fireSlider)));
		Assert.IsNotNull(_plantSlider, Utility.AssertNotNullMessage(nameof(_plantSlider)));
		Assert.IsNotNull(_usesSlider, Utility.AssertNotNullMessage(nameof(_usesSlider)));

		if (!_fireSlider.TryGetComponent(out _fireSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Fire Slider is not a valid ISettable"));

		if (!_plantSlider.TryGetComponent(out _plantSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Plant Slider is not a valid ISettable"));

		if (!_usesSlider.TryGetComponent(out _usesSliderSettable))
			Assert.IsTrue(false, Utility.TraceMessage("Uses Slider is not a valid ISettable"));

		_fireSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.CauldronSettings.FireDuration);
		_plantSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.StartNumberOfPlants);
		_usesSliderSettable.SetMaximum(GameManager.Instance.ActiveLevel.CauldronSettings.MaximumUses);

		Debug.Log("Leaving UIOverlayManager:Start");
	}

	private void Update()
	{
		_usesSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses);
		_plantSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CurrentNumberOfPlants);
		_fireSliderSettable.SetValue(GameManager.Instance.ActiveLevel.CauldronSettings.CurrentFireLevel);
	}
}
