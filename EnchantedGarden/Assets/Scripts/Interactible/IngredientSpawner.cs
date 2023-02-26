using UnityEngine;
using UnityEngine.Assertions;

public class IngredientSpawner : PickUpSpawnerBase
{
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[SerializeField]
	private Color _alertIconColor = Color.yellow;

	private PickUpIndicator _pickUpIndicator;

	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		_pickUpIndicator = GetComponentInChildren<PickUpIndicator>(true);
		Assert.IsNotNull(_pickUpIndicator, Utility.AssertNotNullMessage(nameof(_pickUpIndicator)));

		_pickUpIndicator.SetIconColor(_alertIconColor);

		SubscribeToWorldEvents();
		GameManager.Instance.CheckIngredientsLow();
	}

    private void OnDestroy()
    {
		UnsubscribeFromWorldEvents();
	}

	private void SubscribeToWorldEvents()
	{
		_worldEvents.IngredientsEmpty += IngredientsEmptyWarning;
		_worldEvents.IngredientsFull += IngredientsFull;
	}

	private void UnsubscribeFromWorldEvents()
	{
		_worldEvents.IngredientsEmpty -= IngredientsEmptyWarning;
		_worldEvents.IngredientsFull -= IngredientsFull;
	}

	private void IngredientsEmptyWarning(object _, Vector3 e)
	{
		_pickUpIndicator.SetActive(true);
	}

	private void IngredientsFull(object _, Vector3 e)
	{
		_pickUpIndicator.SetActive(false);
	}
}
