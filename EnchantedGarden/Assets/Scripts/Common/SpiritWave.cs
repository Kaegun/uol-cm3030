using System;

[Serializable]
// Class rather than struct to allow mutability when stored in a queue
public class SpiritWave
{
	//	Number of spirits spawned in the wave
	public int Count;

	//	Delay from previous wave to this wave
	public float Delay;

	// Movement speed multiplier for spirits in this wave
	public float MoveSpeedMultiplier = 1f;

	// Possession rate multiplier for spirits in this wave
	public float PossessionRateMultiplier = 1f;
}