using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

/// <summary>
/// Extended Unity provided Cinemachine Input Provider, to add ability to have mouse down for look.
/// </summary>
public class CinemachineInputProviderEx : CinemachineInputProvider
{
	/// <summary>Button action for looking</summary>
	[Tooltip("Button action for Look Button")]
	[SerializeField]
	private InputActionReference LookButton;

	/// <summary>
	/// Implementation of AxisState.IInputAxisProvider.GetAxisValue().
	/// Axis index ranges from 0...2 for X, Y, and Z.
	/// Reads the action associated with the axis.
	/// </summary>
	/// <param name="axis"></param>
	/// <returns>The current axis value</returns>
	public override float GetAxisValue(int axis)
	{
		if (enabled)
		{
			//	If Action is set, then check that button is pressed, else ignore it.
			if (LookButton != null && LookButton.action != null)
			{
				if (LookButton.action.ReadValue<float>() == 0.0f)
					return 0f;
			}

			return base.GetAxisValue(axis);
		}

		return 0;
	}
}