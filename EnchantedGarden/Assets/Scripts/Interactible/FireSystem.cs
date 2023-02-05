using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FireSystem : MonoBehaviour
{
	[Serializable]
	public class FireSystemParameters
	{
		public float StartLifeTime;
		public float StartSpeed;
		public float StartSize;
	}

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

	private float _currentFireLevel, _fireLifetimeStep;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_fireParticles);

		_currentFireLevel = _fireLifetime;
		_fireLifetimeStep = _fireLifetime / 3;
	}

	// Update is called once per frame
	private void Update()
	{
		//	Set fire animations based on fire timer level
		_currentFireLevel -= Time.deltaTime;

		if (_currentFireLevel < 0)
		{
			//	Fire is dead
			DisableLogs();
			_fireParticles.Stop();
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

	//	TODO: Called when a log has been added - use SO?
	private void LogAdded()
	{
		_currentFireLevel = _fireLifetime;
		SetParticleSystem(_highFireParameters, _highFireLogs);
		//	Restart particles if stopped
		if (_fireParticles.isStopped)
		{
			_fireParticles.Play();
		}
	}
}
