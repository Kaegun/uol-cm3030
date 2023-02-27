using System;
using UnityEngine;

[Serializable]
public class CauldronSettings
{
	[SerializeField]
	private int _startNumberOfUses;
	public int StartNumberOfUses => _startNumberOfUses;

	[SerializeField]
	private int _maximumUses;
	public int MaximumUses => _maximumUses;

	[SerializeField]
	private float _startFireDuration = 30.0f;
	public float StartFireDuration => _startFireDuration;

	[SerializeField]
	private float _fireDuration = 30.0f;
	public float FireDuration => _fireDuration;

	public int CurrentNumberOfUses { get; set; }
	public float CurrentFireLevel { get; set; }
}
