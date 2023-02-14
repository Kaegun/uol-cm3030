using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable/LevelDefinition")]
public class ScriptableLevelDefinition : ScriptableObject
{
	//	The duration of a level
	[SerializeField]
	[Range(15f, 600f)]
	private float _levelDuration = 90f;
	public float LevelDuration { get { return _levelDuration; } }

	//	The definition of the spirit spawning frequency
	[SerializeField]
	private SpiritWave[] _waves;
	public SpiritWave[] Waves { get { return _waves; } }

	//  Factor by how much easier it is to possess the plant
	[SerializeField]
	private float _unplantedFactor = 5f;
	public float UnplantedFactor { get { return _unplantedFactor; } }

	//  Threshold for amount of time plant must be BeingPossessed state before becoming Possessed
	[SerializeField]
	private float _possessionThreshold = 10f;
	public float PossessionThreshold { get { return _possessionThreshold; } }

	//  Threshold for amount of time taken to replant a plant
	[SerializeField]
	private float _replantingThreshold = 2f;
	public float ReplantingThreshold { get { return _replantingThreshold; } }

	[SerializeField]
	private CauldronSettings _cauldronSettings;
}
