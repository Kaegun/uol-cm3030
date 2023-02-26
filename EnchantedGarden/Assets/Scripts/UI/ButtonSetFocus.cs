using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ButtonSetFocus : MonoBehaviour
{
	[SerializeField]
	private Button _focusButton;

	private void Start()
	{
		Assert.IsNotNull(_focusButton, Utility.AssertNotNullMessage(nameof(_focusButton)));

		_focusButton.Select();
	}
}
