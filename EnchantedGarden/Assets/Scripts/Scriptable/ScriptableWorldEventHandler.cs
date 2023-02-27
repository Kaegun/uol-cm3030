using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Events/WorldEvents")]
public class ScriptableWorldEventHandler : ScriptableObject, IEventPublisher
{
	//	Spirit Events
	public event EventHandler<Spirit[]> SpiritWaveSpawned;
	public event EventHandler<Spirit> SpiritSpawned;
	public event EventHandler<Spirit> SpiritWallSpawned;
	public event EventHandler<Spirit> SpiritBanished;
	public event EventHandler<SpiritWall> SpiritWallBanished;

	//	Plant Events
	public event EventHandler<Vector3> PlantPossessed;
	public event EventHandler<Vector3> PlantPossessing;
	public event EventHandler<Vector3> PlantReplanted;
	public event EventHandler<GameObject> PlantStolen;
	public event EventHandler<GameObject> PickUpTrickPlant;
	public event EventHandler<GameObject> PlantDroppedOutOfPatch;

	//	Fire Events
	public event EventHandler<Vector3> FireDied;
	public event EventHandler<Vector3> FireFull;
	public event EventHandler<Vector3> FireMediumWarning;
	public event EventHandler<Vector3> FireLowWarning;

	//	Ingredients
	public event EventHandler<Vector3> IngredientsLowWarning;
	public event EventHandler<Vector3> IngredientsFull;
	public event EventHandler<Vector3> IngredientsEmpty;

	//	Fox
	public event EventHandler<GameObject> FoxAlert;
	public event EventHandler<GameObject> FoxAlertEnded;

	//	Game Events
	public event EventHandler<string> LevelStarted;

	//	Score Events
	public event EventHandler<ScoreEventArguments> Score;

	public void OnSpiritWaveSpawned(Spirit[] spirits)
	{
		this.ExecuteEvent(SpiritWaveSpawned, spirits);
	}

	public void OnSpiritSpawned(Spirit spirit)
	{
		this.ExecuteEvent(SpiritSpawned, spirit);
	}

	public void OnSpiritWallSpawned(Spirit spirit)
	{
		this.ExecuteEvent(SpiritWallSpawned, spirit);
	}

	public void OnSpiritBanished(Spirit spirit)
	{
		this.ExecuteEvent(SpiritBanished, spirit);
	}

	public void OnSpiritWallBanished(SpiritWall spirit)
	{
		this.ExecuteEvent(SpiritWallBanished, spirit);
	}

	public void OnPlantPossessing(Vector3 location)
	{
		this.ExecuteEvent(PlantPossessing, location);
	}

	public void OnPlantPossessed(Vector3 location)
	{
		this.ExecuteEvent(PlantPossessed, location);
	}

	public void OnPlantReplanted(Vector3 location)
	{
		this.ExecuteEvent(PlantReplanted, location);
	}

	public void OnPlantStolen(GameObject plant)
	{
		this.ExecuteEvent(PlantStolen, plant);
	}

	public void OnPlantDroppedOutOfPatch(GameObject plant)
	{
		this.ExecuteEvent(PlantDroppedOutOfPatch, plant);
	}

	public void OnPickUpTrickPlant(GameObject trickPlant)
	{
		this.ExecuteEvent(PickUpTrickPlant, trickPlant);
	}

	public void OnFireDied(Vector3 location)
	{
		this.ExecuteEvent(FireDied, location);
	}

	public void OnFireFull(Vector3 location)
	{
		this.ExecuteEvent(FireFull, location);
	}

	public void OnFireMediumWarning(Vector3 location)
	{
		this.ExecuteEvent(FireMediumWarning, location);
	}

	public void OnFireLowWarning(Vector3 location)
	{
		this.ExecuteEvent(FireLowWarning, location);
	}

	public void OnIngredientsLowWarning(Vector3 location)
	{
		this.ExecuteEvent(IngredientsLowWarning, location);
	}

	public void OnIngredientsFull(Vector3 location)
	{
		this.ExecuteEvent(IngredientsFull, location);
	}

	public void OnIngredientsEmpty(Vector3 location)
	{
		this.ExecuteEvent(IngredientsEmpty, location);
	}

	public void OnFoxAlert(GameObject fox)
	{
		this.ExecuteEvent(FoxAlert, fox);
	}

	public void OnFoxAlertEnded(GameObject fox)
	{
		this.ExecuteEvent(FoxAlertEnded, fox);
	}

	public void OnLevelStarted(string levelName)
	{
		this.ExecuteEvent(LevelStarted, levelName);
	}

	public struct ScoreEventArguments
	{
		public float Score { get; private set; }

		public Vector3 Position { get; private set; }

		public ScoreEventArguments(float score, Vector3 position)
		{
			Score = score;
			Position = position;
		}
	}

	public void OnScore(ScoreEventArguments scoreEventArguments)
	{
		this.ExecuteEvent(Score, scoreEventArguments);
	}
}
