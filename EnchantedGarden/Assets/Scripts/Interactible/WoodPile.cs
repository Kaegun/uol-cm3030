using UnityEngine;
using UnityEngine.Assertions;

public class WoodPile : PickUpSpawnerBase
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
	}

    private void OnDestroy()
    {
		UnsubscribeFromWorldEvents();
	}

	private void SubscribeToWorldEvents()
	{
		_worldEvents.FireDied += FireDied;
		_worldEvents.FireFull += FireFull;
	}

	private void UnsubscribeFromWorldEvents()
	{
		_worldEvents.FireDied -= FireDied;
		_worldEvents.FireFull -= FireFull;
	}

	private void FireDied(object _, Vector3 e)
	{
		_pickUpIndicator.SetActive(true);
	}

	private void FireFull(object _, Vector3 e)
	{
		_pickUpIndicator.SetActive(false);
	}
}
