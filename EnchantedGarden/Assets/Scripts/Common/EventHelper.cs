using System;

public static class EventHelper
{
	public static void ExecuteEvent<T>(this IEventPublisher publisher, EventHandler<T> handler, T e)
	{
		if (handler != null)
		{
			foreach (var evt in handler.GetInvocationList())
			{
				evt.DynamicInvoke(publisher, e);
			}
		}
	}
}

