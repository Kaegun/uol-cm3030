using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Events/WorldEvents")]
public class ScriptableWorldEventHandler : ScriptableEventHandler
{
	//	Spirit Events
	public event EventHandler<Spirit[]> SpiritWaveSpawned;
	public event EventHandler<Spirit> SpiritWallSpawned;
	public event EventHandler<Spirit> SpiritBanished;

	//	Plant Events
	public event EventHandler<Vector3> PlantPossessed;
	public event EventHandler<Vector3> PlantPossessing;
	public event EventHandler<Vector3> PlantStolen;

	//	Fire Events
	public event EventHandler<Vector3> FireDied;
	public event EventHandler<Vector3> FireMediumWarning;
	public event EventHandler<Vector3> FireLowWarning;

	//	Low Ingredients
	public event EventHandler<Vector3> IngredientsLowWarning;

	//	Fox
	public event EventHandler<GameObject> FoxAlert;
	public event EventHandler<GameObject> FoxAlertEnded;

	public void OnSpiritWaveSpawned(Spirit[] spawnLocations)
	{
		ExecuteEvent(SpiritWaveSpawned, spawnLocations);
	}

	public void OnSpiritWallSpawned(Spirit location)
	{
		ExecuteEvent(SpiritWallSpawned, location);
	}

	public void OnSpiritBanished(Spirit location)
	{
		ExecuteEvent(SpiritBanished, location);
	}
	public void OnPlantPossessing(Vector3 location)
	{
		ExecuteEvent(PlantPossessing, location);
	}

	public void OnPlantPossessed(Vector3 location)
	{
		ExecuteEvent(PlantPossessed, location);
	}

	public void OnPlantStolen(Vector3 location)
	{
		ExecuteEvent(PlantStolen, location);
	}

	public void OnFireDied(Vector3 location)
	{
		ExecuteEvent(FireDied, location);
	}

	public void OnFireMediumWarning(Vector3 location)
	{
		ExecuteEvent(FireMediumWarning, location);
	}

	public void OnFireLowWarning(Vector3 location)
	{
		ExecuteEvent(FireLowWarning, location);
	}

	public void OnIngredientsLowWarning(Vector3 location)
	{
		ExecuteEvent(IngredientsLowWarning, location);
	}

	public void OnFoxAlert(GameObject location)
	{
		ExecuteEvent(FoxAlert, location);
	}

	public void OnFoxAlertEnded(GameObject location)
	{
		ExecuteEvent(FoxAlertEnded, location);
	}
}
