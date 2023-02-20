using System;
using UnityEngine;

public class Flask : PickUpBase, ICombinable, IInteractor
{
	[SerializeField]
	private GameObject _contents;

	[SerializeField]
	private bool _full;

	[SerializeField]
	private float _combinationThreshold = 1f;
	public float CombinationThreshold => _combinationThreshold;

	public event EventHandler<float> CombineProgress;

	[SerializeField]
	private ScriptableAudioClip _flaskSmashAudio;

	public bool CanUseFlask => _full;

	private float _combinationProgress = 0f;

	private void UseFlask()
	{
		_full = false;
		_contents.SetActive(false);
		AudioController.PlayAudioDetached(_flaskSmashAudio, transform.position);
	}

	public bool Combining()
	{
		_combinationProgress += Time.deltaTime;
		ExecuteEvent(CombineProgress, _combinationProgress);

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
		_combinationProgress = 0f;

		//  Reduce the number of uses in the Cauldron - JU: Not a fan
		GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses--;
	}

	public bool CanBeCombined => _held && !_full;

	public GameObject GameObject => gameObject;

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

	private void ExecuteEvent<T>(EventHandler<T> handler, T e)
	{
		if (handler != null)
		{
			foreach (var evt in handler.GetInvocationList())
			{
				evt.DynamicInvoke(this, e);
			}
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		_full = false;
		_contents.SetActive(false);
	}
}
