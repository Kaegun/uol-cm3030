using System;
using UnityEngine;

public abstract class ScriptableEventHandler : ScriptableObject
{
	protected void ExecuteEvent<T>(EventHandler<T> handler, T e)
	{
		if (handler != null)
		{
			foreach (var evt in handler.GetInvocationList())
			{
				evt.DynamicInvoke(this, e);
			}
		}
	}
}
