using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Scriptable/Events/InputEvents")]
public class ScriptableInputEventHandler : ScriptableObject, IEventPublisher
{
	public event EventHandler<float> InteractionPressed;
	public event EventHandler<float> InteractionReleased;
	public event EventHandler<Vector2> Movement;

	public void OnInteractionPressed(InputAction.CallbackContext context)
	{
		if (HandleInputDown(context, out float pressed))
			this.ExecuteEvent(InteractionPressed, pressed);

		if (HandleInputUp(context, out float up))
			this.ExecuteEvent(InteractionReleased, up);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (HandleInputDown(context, out Vector2 movement) || HandleInputUp(context, out movement))
			this.ExecuteEvent(Movement, movement);
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
