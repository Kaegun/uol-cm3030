using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class FireSystem : MonoBehaviour
{
	[Serializable]
	public class FireSystemParameters
	{
		public float StartLifeTime;
		public float StartSpeed;
		public float StartSize;
	}

	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _fireAmbientAudio;

	[SerializeField]
	private ScriptableAudioClip _fireAddLogAudio;

	[Header("Fire particles")]
	[SerializeField]
	private float _fireLifetime = 10.0f;

	[SerializeField]
	private ParticleSystem _fireParticles;

	[Header("High")]
	[SerializeField]
	private FireSystemParameters _highFireParameters;

	[SerializeField]
	private GameObject _highFireLogs;

	[Header("Medium")]
	[SerializeField]
	private FireSystemParameters _mediumFireParameters;

	[SerializeField]
	private GameObject _mediumFireLogs;

	[Header("Low")]
	[SerializeField]
	private FireSystemParameters _lowFireParameters;

	[SerializeField]
	private GameObject _lowFireLogs;

	public bool IsAlive => _currentFireLevel > 0;

	private AudioSource _fireAudioSource;
	private float _currentFireLevel, _fireLifetimeStep;

	public void AddLog()
	{
		Debug.Log("Entering AddLog");
		_currentFireLevel = _fireLifetime;
		AudioController.PlayAudio(_fireAudioSource, _fireAddLogAudio);
		_currentFireLevel = _fireLifetime;
		SetParticleSystem(_highFireParameters, _highFireLogs);
		//	Restart particles if stopped
		if (_fireParticles.isStopped)
		{
			_fireParticles.Play();
			//	Play ambient audio - maybe delayed via CoR?
			StartCoroutine(StartAmbientAudioCoRoutine());
		}
	}

	private IEnumerator StartAmbientAudioCoRoutine()
	{
		yield return new WaitForSeconds(_fireAddLogAudio.clip.length * 0.6f);
		// Play fire ambient noise
		AudioController.PlayAudio(_fireAudioSource, _fireAmbientAudio);
	}

	//  TODO: Does this need to be in a CoRoutine?
	//private IEnumerator FireCoroutine()
	//{
	//	_currentFuel = _maxFuel;
	//	_fireParticles.SetActive(true);
	//	// Play add log to fire noise
	//	AudioController.PlayAudio(_fireAudioSource, _fireAddLogAudio);
	//	yield return new WaitForSeconds(_fireAddLogAudio.clip.length * 0.6f);
	//	// Play fire ambient noise
	//	AudioController.PlayAudio(_fireAudioSource, _fireAmbientAudio);
	//	yield return new WaitForSeconds(_maxFuel - _fireAddLogAudio.clip.length * 0.6f);
	//	// Stop fire
	//	_fireAudioSource.Stop();
	//	_fireParticles.SetActive(false);
	//	_currentFuel = 0;
	//}

	// Start is called before the first frame update
	private void Start()
	{
		_fireAudioSource = GetComponent<AudioSource>();
		Assert.IsNotNull(_fireAudioSource);
		Assert.IsNotNull(_fireParticles);

		_currentFireLevel = _fireLifetime;
		_fireLifetimeStep = _fireLifetime / 3;

		//	Start Fire Audio
		AudioController.PlayAudio(_fireAudioSource, _fireAmbientAudio);
	}

	// Update is called once per frame
	private void Update()
	{
		//	Set fire animations based on fire timer level
		_currentFireLevel -= Time.deltaTime;

		if (!IsAlive)
		{
			//	Fire is dead
			DisableLogs();
			_fireParticles.Stop();
			_fireAudioSource.Stop();
		}
		else if (_currentFireLevel < _fireLifetime - _fireLifetimeStep * 2)
		{
			SetParticleSystem(_lowFireParameters, _lowFireLogs);
		}
		else if (_currentFireLevel < _fireLifetime - _fireLifetimeStep)
		{
			SetParticleSystem(_mediumFireParameters, _mediumFireLogs);
		}
	}

	private void SetParticleSystem(FireSystemParameters parameters, GameObject logs)
	{
		var psMain = _fireParticles.main;
		psMain.startSize = parameters.StartSize;
		psMain.startLifetime = parameters.StartLifeTime;
		psMain.startSpeed = parameters.StartSpeed;
		DisableLogs();

		logs.SetActive(true);
	}

	private void DisableLogs()
	{
		_lowFireLogs.SetActive(false);
		_mediumFireLogs.SetActive(false);
		_highFireLogs.SetActive(false);
	}
}
