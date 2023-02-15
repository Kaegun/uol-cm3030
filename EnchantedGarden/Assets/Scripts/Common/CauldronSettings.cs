using System;
using UnityEngine;

[Serializable]
public class CauldronSettings
{
	[SerializeField]
	private int _maximumUses;
	public int MaximumUses => _maximumUses;

	[SerializeField]
	private float _fireDuration = 30.0f;
	public float FireDuration => _fireDuration;

	public int NumberOfUses { get; set; }

	public CauldronSettings()
	{
		NumberOfUses = _maximumUses;
	}
}
