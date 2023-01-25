using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Scriptable/Events/InputEvents")]
public class ScriptableInputEventHandler : ScriptableEventHandler
{
	public event EventHandler<float> InteractionPressed;
	public event EventHandler<float> InteractionReleased;
	public event EventHandler<Vector2> Movement;

	public void OnInteractionPressed(InputAction.CallbackContext context)
	{
		if (HandleInputDown(context, out float pressed))
			ExecuteEvent(InteractionPressed, pressed);
	}

	public void OnInteractionReleased(InputAction.CallbackContext context)
	{
		if (HandleInputUp(context, out float pressed))
			ExecuteEvent(InteractionReleased, pressed);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 movement;
		if (HandleInputDown(context, out movement) || HandleInputUp(context, out movement))
			ExecuteEvent(Movement, movement);
	}

	private bool HandleInputDown<T>(InputAction.CallbackContext context, out T result) where T : struct
	{
		result = default;
		if (context.phase == InputActionPhase.Performed)
		{
			result = context.ReadValue<T>();
			return true;
		}
		return false;
	}

	private bool HandleInputUp<T>(InputAction.CallbackContext context, out T result) where T : struct
	{
		result = default;
		if (context.phase == InputActionPhase.Canceled)
		{
			result = context.ReadValue<T>();
			return true;
		}
		return false;
	}
}
