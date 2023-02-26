﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class Cauldron : MonoBehaviour, IInteractable
{
	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _cauldronBubbleAudio;

	[SerializeField]
	private ScriptableAudioClip _cauldronCombineAudio;

	[Header("Particles")]
	[SerializeField]
	private GameObject _cauldronContents;

	[SerializeField]
	private GameObject _cauldronContentsParticles;

	[Header("Canvas")]
	[SerializeField]
	private Canvas _cauldronCanvas;

	private int _maxUses;
	private FireSystem _fireSystem;
	private AudioSource _cauldronAudioSource;

	private bool CanUseCauldron => GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses > 0 && _fireSystem.IsAlive;

	public Transform Transform => transform;

	private void AddLog()
	{
		_fireSystem.AddLog();
	}

	private void AddIngredient()
	{
		if (_fireSystem.IsAlive)
		{
			GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses = _maxUses;
			_worldEvents.OnIngredientsFull(transform.position);
			StartCoroutine(CauldronCombineCoroutine());
		}
		else
		{
			StartCoroutine(CannotAddIngredientCoroutine());
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		_fireSystem = GetComponentInChildren<FireSystem>();
		Assert.IsNotNull(_fireSystem, Utility.AssertNotNullMessage(nameof(_fireSystem)));
		Assert.IsTrue(TryGetComponent(out _cauldronAudioSource), Utility.AssertNotNullMessage(nameof(_cauldronAudioSource)));
		Assert.IsNotNull(_cauldronCanvas, Utility.AssertNotNullMessage(nameof(_cauldronCanvas)));
		_cauldronCanvas.gameObject.SetActive(false);

		_maxUses = GameManager.Instance.ActiveLevel.CauldronSettings.MaximumUses;
		GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses = GameManager.Instance.ActiveLevel.CauldronSettings.StartNumberOfUses;

		AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
	}

	// Update is called once per frame
	private void Update()
	{
		if (!CanUseCauldron)
		{
			_cauldronAudioSource.Stop();
			if (GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses > 0)
			{
				_cauldronContentsParticles.SetActive(false);
			}
			else
			{
				_cauldronContents.SetActive(false);
			}

		}
		if (CanUseCauldron)
		{
			if (!_cauldronAudioSource.isPlaying)
			{
				AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
			}
			if (!_cauldronContents.activeSelf)
			{
				_cauldronContents.SetActive(true);
			}
			if (!_cauldronContentsParticles.activeSelf)
			{
				_cauldronContentsParticles.SetActive(true);
			}
		}
	}

	private IEnumerator CauldronCombineCoroutine()
	{
		AudioController.PlayAudio(_cauldronAudioSource, _cauldronCombineAudio);
		yield return new WaitForSeconds(_cauldronCombineAudio.clip.length * 0.8f);
		AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
	}

	private IEnumerator CannotAddIngredientCoroutine()
	{
		_cauldronCanvas.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		_cauldronCanvas.gameObject.SetActive(false);
	}

	public bool CanInteractWith(IInteractor interactor)
	{
		switch (interactor)
		{
			case ICombinable _:
				return interactor.CanInteractWith(this) && CanUseCauldron;
			case Log _:
				return interactor.CanInteractWith(this);
			case Ingredient _:
				return interactor.CanInteractWith(this);
			default:
				return false;
		}
	}

	public void OnInteractWith(IInteractor interactor)
	{
		switch (interactor)
		{
			case ICombinable combinable:
				if (combinable.Combining())
				{
					StartCoroutine(CauldronCombineCoroutine());
					GameManager.Instance.ActiveLevel.CauldronSettings.CurrentNumberOfUses--;
					GameManager.Instance.CheckIngredientsLow();
					GameManager.Instance.CheckIngredientsEmpty();
				}
				break;
			case Log _:
				AddLog();
				break;
			case Ingredient _:
				AddIngredient();
				break;
			default:
				break;
		}
	}

	public bool DestroyOnInteract(IInteractor interactor)
	{
		return false;
	}

	public void DestroyInteractable()
	{
		//	Do nothing
	}
}
