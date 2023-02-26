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

	public void OnSpiritWallBanished(SpiritWall spirit)
	{
		ExecuteEvent(SpiritWallBanished, spirit);
	}

	public void OnPlantPossessing(Vector3 location)
	{
		ExecuteEvent(PlantPossessing, location);
	}

	public void OnPlantPossessed(Vector3 location)
	{
		ExecuteEvent(PlantPossessed, location);
	}

	public void OnPlantReplanted(Vector3 location)
    {
		ExecuteEvent(PlantReplanted, location);
    }

	public void OnPlantStolen(GameObject plant)
	{
		ExecuteEvent(PlantStolen, plant);
	}

	public void OnPlantDroppedOutOfPatch(GameObject plant)
	{
		ExecuteEvent(PlantDroppedOutOfPatch, plant);
	}

	public void OnPickUpTrickPlant(GameObject trickPlant)
	{
		ExecuteEvent(PickUpTrickPlant, trickPlant);
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

	public void OnIngredientsEmpty(Vector3 location)
	{
		ExecuteEvent(IngredientsEmpty, location);
	}

	public void OnFoxAlert(GameObject fox)
	{
		ExecuteEvent(FoxAlert, fox);
	}

	public void OnFoxAlertEnded(GameObject fox)
	{
		ExecuteEvent(FoxAlertEnded, fox);
	}

	public void OnLevelStarted(string levelName)
	{
		ExecuteEvent(LevelStarted, levelName);
	}

	public struct ScoreEventArguments
    {
		public ScoreEventArguments(float score, Vector3 position)
        {
			_score = score;
			_position = position;
        }
		private float _score;
		public float Score => _score;

		private Vector3 _position;
		public Vector3 Position => _position;
    }

	public void OnScore(ScoreEventArguments scoreEventArguments)
	{
		ExecuteEvent(Score, scoreEventArguments);
	}
}
