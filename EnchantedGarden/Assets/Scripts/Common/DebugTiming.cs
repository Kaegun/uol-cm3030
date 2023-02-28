using System;
using UnityEngine;

public class DebugTiming : IDisposable
{
	private readonly float _start;
	private readonly string _name;

	public DebugTiming(string name)
	{
		_start = Time.realtimeSinceStartup;
		_name = name;
	}

	public void Dispose()
	{
		Debug.Log($"[{_name}] Duration: {Time.realtimeSinceStartup - _start}");
	}
}
