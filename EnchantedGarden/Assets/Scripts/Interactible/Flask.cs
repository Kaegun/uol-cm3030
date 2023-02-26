using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Flask : PickUpBase, ICombinable, IInteractor, IEventPublisher
{
	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Contents")]
	[SerializeField]
	private GameObject _contents;

	[SerializeField]
	private bool _full;

	[SerializeField]
	private float _combinationThreshold = 1f;
	public float CombinationThreshold => _combinationThreshold;

	public event EventHandler<float> CombineProgress;

	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _flaskSmashAudio;

	public bool CanUseFlask => _full;

	public bool CanBeCombined => _held && !_full;

	private float _combinationProgress = 0f;

	public override void OnPickUp(Transform pickupTransform)
	{
		base.OnPickUp(pickupTransform);
		this.ExecuteEvent(CombineProgress, _combinationProgress);
	}

	private void UseFlask()
	{
		_full = false;
		_contents.SetActive(false);
		AudioController.PlayAudioDetached(_flaskSmashAudio, transform.position);
	}

	public bool Combining()
	{
		_combinationProgress += Time.deltaTime;
		this.ExecuteEvent(CombineProgress, _combinationProgress);

		if (_combinationProgress >= _combinationThreshold)
		{
			OnCombine();
			return true;
		}

		return false;
	}

	public void OnCombine()
	{
		_full = true;
		_contents.SetActive(true);
		_combinationProgress = _combinationThreshold;
	}

	public bool CanInteractWith(IInteractable interactable)
	{
		switch (interactable)
		{
			case Cauldron _:
				return CanBeCombined;
			case Spirit _:
				return CanUseFlask;
			default:
				return false;
		}
	}

	public void OnInteract(IInteractable interactable)
	{
		switch (interactable)
		{
			case Spirit _:
				UseFlask();
				break;
			default:
				break;
		}
	}

	public bool DestroyAfterInteract(IInteractable interactable)
	{
		switch (interactable)
		{
			case Spirit _:
				return true;
			default:
				return false;
		}
	}

	public void DestroyInteractor()
	{
		Destroy(gameObject);
	}

	// Start is called before the first frame update
	protected override void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));

		_full = false;
		_contents.SetActive(false);
		base.Start();
	}
}
