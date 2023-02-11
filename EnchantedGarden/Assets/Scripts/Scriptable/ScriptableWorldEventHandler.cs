using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Events/WorldEvents")]
public class ScriptableWorldEventHandler : ScriptableEventHandler
{
	//	Plant Events
	public event EventHandler<Vector3[]> SpiritWaveSpawned;
	public event EventHandler<Vector3> PlantPossessed;
	public event EventHandler<Vector3> PlantPossessing;

	//	UI Events
	public event EventHandler<ScriptableLevelDefinition> UiSceneLoaded;

	public void OnSpiritWaveSpawned(Vector3[] spawnLocations)
	{
		ExecuteEvent(SpiritWaveSpawned, spawnLocations);
	}

	public void OnPlantPossessing(Vector3 location)
	{
		ExecuteEvent(PlantPossessing, location);
	}

	public void OnPlantPossessed(Vector3 location)
	{
		ExecuteEvent(PlantPossessed, location);
	}

	public void OnUiSceneLoaded(ScriptableLevelDefinition level)
	{
		ExecuteEvent(UiSceneLoaded, level);
	}
}
