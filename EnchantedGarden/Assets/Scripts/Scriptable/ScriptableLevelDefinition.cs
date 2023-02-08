using System;
using UnityEngine;

//  TODO: Do not use embedded classes
//  TODO: This is a good use case for a scriptable object -> One per level
[Serializable]
// Class rather than struct to allow mutability when stored in a queue
public class SpiritWave
{
	// number of spirits spawned in the wave
	public int Count;

	// delay from previous wave to this wave
	public float Delay;
}

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
}
