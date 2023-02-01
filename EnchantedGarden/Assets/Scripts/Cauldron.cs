using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [SerializeField]
    private AudioSource _fireAudioSource;
    [SerializeField]
    private ScriptableAudioClip _fireAmbientAudio;
    [SerializeField]
    private ScriptableAudioClip _fireAddLogAudio;

    [SerializeField]
    private AudioSource _cauldronAudioSource;
    [SerializeField]
    private ScriptableAudioClip _cauldronCombineAudio;
    [SerializeField]
    private ScriptableAudioClip _cauldronBubbleAudio;

    [SerializeField]
    private GameObject _fireParticles;

    [SerializeField]
    private float _maxFuel = 10f;
    private float _currentFuel;

    [SerializeField]
    private int _maxUses = 5;
    private int _currentUses;

    [SerializeField]
    private float _combineDuration;
    private float _combineProgress;

    [SerializeField]
    private GameObject _progressDots;

    [SerializeField]
    private TextMeshPro _usesText;

    private Coroutine _fireCoroutine;

    public void AddLog()
    {
        // Restart FireCoroutine
        StopCoroutine(_fireCoroutine);
        _fireCoroutine = StartCoroutine(FireCoroutine());
    }

    public void AddHerb()
    {
        _currentUses = _maxUses;
        _usesText.text = $"{_currentUses}/{_maxUses}";
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentUses = _maxUses;
        _fireCoroutine = StartCoroutine(FireCoroutine());
        AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
    }

    // Update is called once per frame
    void Update()
    {
        //	Could we use a Trigger Collider here?
        var combinables = Physics.OverlapSphere(transform.position, 2f).
            Where(c => c.GetComponent<ICombinable>() != null && c.GetComponent<ICombinable>().CanBeCombined()).
            Select(c => c.GetComponent<ICombinable>()).
            ToList();

        //	Could this be done in a Co-Routine?
        if (combinables.Count > 0 && CanUseCauldron())
        {
            _progressDots.SetActive(true);
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
            _progressDots.SetActive(false);
            _combineProgress = 0;
        }
    }

    private bool CanUseCauldron()
    {
        return _currentUses > 0 && _currentFuel > 0;
    }

    private void UsePotion()
    {
        _currentUses -= 1;
        _usesText.text = $"{_currentUses}/{_maxUses}";
    }

    private IEnumerator FireCoroutine()
    {
        _currentFuel = _maxFuel;
        _fireParticles.SetActive(true);
        // Play add log to fire noise
        AudioController.PlayAudio(_fireAudioSource, _fireAddLogAudio);
        yield return new WaitForSeconds(_fireAddLogAudio.clip.length * 0.6f);
        // Play fire ambient noise
        AudioController.PlayAudio(_fireAudioSource, _fireAmbientAudio);
        yield return new WaitForSeconds(_maxFuel - _fireAddLogAudio.clip.length * 0.6f);
        // Stop fire
        _fireAudioSource.Stop();
        _fireParticles.SetActive(false);
        _currentFuel = 0;
    }

    private IEnumerator CauldronCombineCoroutine()
    {
        AudioController.PlayAudio(_cauldronAudioSource, _cauldronCombineAudio);
        yield return new WaitForSeconds(_cauldronCombineAudio.clip.length * 0.8f);
        AudioController.PlayAudio(_cauldronAudioSource, _cauldronBubbleAudio);
    }
}
