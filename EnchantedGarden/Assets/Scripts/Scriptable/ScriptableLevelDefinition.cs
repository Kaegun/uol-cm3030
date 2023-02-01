using UnityEngine;

//  TODO: Do not use embedded classes
//  TODO: This is a good use case for a scriptable object -> One per level
[System.Serializable]
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
	//	The definition of the spirit spawning frequency
	[SerializeField]
	private SpiritWave[] _waves;
	public SpiritWave[] Waves { get { return _waves; } }

	//	The duration of a level
	[SerializeField]
	private float _levelDuration;
	public float LevelDuration { get { return _levelDuration; } }
}
