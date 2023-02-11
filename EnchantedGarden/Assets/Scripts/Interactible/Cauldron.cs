using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class Cauldron : MonoBehaviour
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
	private float _combineDuration;
	private float _combineProgress;

	//	TODO: Build a UI component
	//[SerializeField]
	//private GameObject _progressDots;

	//[SerializeField]
	//private TextMeshPro _usesText;

	private FireSystem _fireSystem;
	private AudioSource _cauldronAudioSource;

	public void AddLog()
	{
		_fireSystem.AddLog();
	}

	public void AddHerb()
	{
		_currentUses = _maxUses;
		//_usesText.text = $"{_currentUses}/{_maxUses}";
	}

	public void FillPesticideSpray(PesticideSpray pesticideSpray)
	{
		//	Do pesticide stuff
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
		//	TODO: Could we use a Trigger Collider here?
		var combinables = Physics.OverlapSphere(transform.position, 2f)
			.Where(c => c.GetComponent<ICombinable>() != null && c.GetComponent<ICombinable>().CanBeCombined)
			.Select(c => c.GetComponent<ICombinable>())
			.ToList();

		//	Could this be done in a Co-Routine?
		if (combinables.Count > 0 && CanUseCauldron)
		{
			//_progressDots.SetActive(true);
			_combineProgress += Time.deltaTime;
			if (_combineProgress >= _combineDuration)
			{
				UsePotion();
				combinables[0].OnCombine();
				_combineProgress = 0;
				StartCoroutine(CauldronCombineCoroutine());
			}
		}
		else
		{
			//_progressDots.SetActive(false);
			_combineProgress = 0;
		}
	}

	private bool CanUseCauldron => _currentUses > 0 && _fireSystem.IsAlive;

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
}
