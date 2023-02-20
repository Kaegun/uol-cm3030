using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Events/WorldEvents")]
public class ScriptableWorldEventHandler : ScriptableEventHandler
{
	//	Spirit Events
	public event EventHandler<Spirit[]> SpiritWaveSpawned;
	public event EventHandler<Spirit> SpiritSpawned;
	public event EventHandler<Spirit> SpiritWallSpawned;
	public event EventHandler<Spirit> SpiritBanished;

	//	Plant Events
	public event EventHandler<Vector3> PlantPossessed;
	public event EventHandler<Vector3> PlantPossessing;
	public event EventHandler<GameObject> PlantStolen;

	//	Fire Events
	public event EventHandler<Vector3> FireDied;
	public event EventHandler<Vector3> FireFull;
	public event EventHandler<Vector3> FireMediumWarning;
	public event EventHandler<Vector3> FireLowWarning;

	//	Ingredients
	public event EventHandler<Vector3> IngredientsLowWarning;
	public event EventHandler<Vector3> IngredientsFull;

	//	Fox
	public event EventHandler<GameObject> FoxAlert;
	public event EventHandler<GameObject> FoxAlertEnded;

	public void OnSpiritWaveSpawned(Spirit[] spirits)
	{
		ExecuteEvent(SpiritWaveSpawned, spirits);
	}

	public void OnSpiritSpawned(Spirit spirit)
	{
		ExecuteEvent(SpiritSpawned, spirit);
	}

	public void OnSpiritWallSpawned(Spirit spirit)
	{
		ExecuteEvent(SpiritWallSpawned, spirit);
	}

	public void OnSpiritBanished(Spirit spirit)
	{
		ExecuteEvent(SpiritBanished, spirit);
	}
	public void OnPlantPossessing(Vector3 location)
	{
		ExecuteEvent(PlantPossessing, location);
	}

	public void OnPlantPossessed(Vector3 location)
	{
		ExecuteEvent(PlantPossessed, location);
	}

	public void OnPlantStolen(GameObject plant)
	{
		ExecuteEvent(PlantStolen, plant);
	}

	public void OnFireDied(Vector3 location)
	{
		ExecuteEvent(FireDied, location);
	}

	public void OnFireFull(Vector3 location)
	{
		ExecuteEvent(FireFull, location);
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

	public void OnIngredientsFull(Vector3 location)
	{
		ExecuteEvent(IngredientsFull, location);
	}

	public void OnFoxAlert(GameObject fox)
	{
		ExecuteEvent(FoxAlert, fox);
	}

	public void OnFoxAlertEnded(GameObject fox)
	{
		ExecuteEvent(FoxAlertEnded, fox);
	}
}
