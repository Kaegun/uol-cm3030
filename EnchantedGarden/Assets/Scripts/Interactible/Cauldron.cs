using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class Cauldron : MonoBehaviour, IInteractable
{
	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _cauldronBubbleAudio;

	[SerializeField]
	private ScriptableAudioClip _cauldronCombineAudio;

	[SerializeField]
	private int _maxUses = 5;
	private int _currentUses;

	[SerializeField]
	private GameObject _cauldronContents;
	[SerializeField]
	private GameObject _cauldronContentsParticles;	

	private FireSystem _fireSystem;
	private AudioSource _cauldronAudioSource;

	private void AddLog()
	{
		_fireSystem.AddLog();
	}

	private void AddIngredient()
	{
		_currentUses = _maxUses;
		if (_fireSystem.IsAlive)
		{			
			StartCoroutine(CauldronCombineCoroutine());
		}
	}	

	// Start is called before the first frame update
	private void Start()
	{
		_fireSystem = GetComponentInChildren<FireSystem>();
		Assert.IsNotNull(_fireSystem);
		if (!TryGetComponent(out _cauldronAudioSource))
		{
			Assert.IsNotNull(_cauldronAudioSource);
		}

		_currentUses = _maxUses;
		AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
	}

	// Update is called once per frame
	private void Update()
	{
		if (!CanUseCauldron)
        {
			_cauldronAudioSource.Stop();
			if (_currentUses > 0)
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

	private bool CanUseCauldron => _currentUses > 0 && _fireSystem.IsAlive;

    public Transform Transform => transform;

    public GameObject GameObject => gameObject;

    private void UsePotion()
	{
		_currentUses -= 1;
		//_usesText.text = $"{_currentUses}/{_maxUses}";
	}

	private IEnumerator CauldronCombineCoroutine()
	{
		AudioController.PlayAudio(_cauldronAudioSource, _cauldronCombineAudio);
		yield return new WaitForSeconds(_cauldronCombineAudio.clip.length * 0.8f);
		AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
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
}
