using System.Linq;
using UnityEngine;

public class FoxBehaviour : MonoBehaviour
{
	/*
	 * This behaviour must listen for world events, and then instruct the Fox to react to them
	 * Test 1 - When a spirit Spawns
	 * 
	 * 
	 */

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	// Start is called before the first frame update
	private void Start()
	{
		_worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
		_worldEvents.PlantPossessing += PlantPossessing;
		_worldEvents.PlantPossessed += PlantPossessed;
	}

	// Update is called once per frame
	private void Update()
	{

	}

	private void PlantPossessing(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		Debug.Log($"Fox Behaviour: Plant Possessing - [{e}]");
	}

	private void PlantPossessed(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		//	More insistent
		//	Run to the bug sprayer
		Debug.Log($"Fox Behaviour: Plant Possessed - [{e}]");
	}

	private void SpiritWaveSpawned(object sender, Vector3[] e)
	{
		//	The fox might not do much here
		//	Could also use the camera for some of it
		Debug.Log($"Fox Behaviour: Spirit Wave Spawned - [{e.Length}]");
	}
}
