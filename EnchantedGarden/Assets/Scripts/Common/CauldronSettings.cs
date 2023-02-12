using System;
using UnityEngine;

[Serializable]
public class CauldronSettings
{
	[SerializeField]
	private int _maximumUses;
	public int MaximumUses => _maximumUses;

	public int NumberOfUses { get; set; }

	public CauldronSettings()
	{
		NumberOfUses = _maximumUses;
	}
}
