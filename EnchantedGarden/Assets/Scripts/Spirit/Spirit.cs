using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Spirit : MonoBehaviour, IInteractable
{
	public enum SpiritState
	{
		Spawning,
		Searching,
		StartingPossession,
		Possessing,
		Trapped,
		Banished,
	}

	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Movement")]
	[SerializeField]
	private float _moveSpeed;

	[SerializeField]
	private float _turnSpeed = 10f;

	[SerializeField]
	private GameObject _bodyObj;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private ScriptableAudioClip _spawnAudio;

	[SerializeField]
	private ScriptableAudioClip _beginPossessingAudio;

	[SerializeField]
	private ScriptableAudioClip _completePossessionAudio;

	[SerializeField]
	private ScriptableAudioClip _bansihAudio;

	[Header("VFX")]
	[SerializeField]
	private Material _banishableMaterial;

	[SerializeField]
	private Material _banishedMaterial;

	[Header("Misc")]
	[SerializeField]
	private bool _tutorialSpirit = false;

	[SerializeField]
	private float _tutorialSpiritWaitDuration = 4f;

	private SpiritBody _spiritBody;
	private Vector3 _moveDirection;
	private float _moveSpeedMultiplier = 1f;
	private float _possessionRateMultiplier = 1f;
	public float PossessionRateMultiplier => _possessionRateMultiplier;

	private SpiritState _spiritState;
	public SpiritState ActiveSpiritState => _spiritState;
	private IPossessable _possessedPossessable;
	private Vector3 _spawnPos;
	private IPossessable _targetPossessable = null;

	private const float _possessedSpeedFactor = 0.33f,
		_justSpawnedMovementFactor = 0.5f,
		_spawnMovementDelay = 2.0f,
		_widenSearchThreshold = 7.5f,
		_banishTimeout = 0.8f;

	public bool CanBeBanished => _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;

	public void SetPropertiesOnSpawn(float moveSpeedMultiplier, float possessionRateMultiplier)
	{
		_moveSpeedMultiplier = moveSpeedMultiplier;
		_possessionRateMultiplier = possessionRateMultiplier;
	}

	private bool IsPossessingPlant => (_spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession) && _possessedPossessable is Plant;

	public Transform Transform => transform;

	public bool CanInteractWith(IInteractor interactor)
	{
		switch (interactor)
		{
			case Flask _:
				return CanBeBanished && interactor.CanInteractWith(this);
			default:
				return false;
		}
	}

	public void OnInteractWith(IInteractor interactor)
	{
		switch (interactor)
		{
			case Flask _:
				Banish();
				break;
			default:
				break;
		}
	}

	public bool DestroyOnInteract(IInteractor interactor)
	{
		switch (interactor)
		{
			case Flask _:
				return true;
			default:
				return false;
		}
	}

	public void DestroyInteractable()
	{
		//	Wait for the effect to finish
		Destroy(gameObject, _banishTimeout);
	}

	private void StealPossessedPlant()
	{
		//  Alert the world a plant is dead
		_worldEvents.OnPlantStolen(_possessedPossessable.GameObject);

		Destroy(_possessedPossessable.GameObject);
		Destroy(gameObject);
	}

	private void Banish()
	{
		_spiritBody.SetMaterial(_banishedMaterial, false);
		_possessedPossessable?.OnDispossess();
		_worldEvents.OnSpiritBanished(this);
		// Spirit is destroyed before audio plays
		AudioController.PlayAudioDetached(_bansihAudio, transform.position);
		_spiritState = SpiritState.Banished;
	}

	//  Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		Assert.IsNotNull(_banishableMaterial, Utility.AssertNotNullMessage(nameof(_banishableMaterial)));
		Assert.IsNotNull(_banishedMaterial, Utility.AssertNotNullMessage(nameof(_banishedMaterial)));
		_spiritBody = GetComponentInChildren<SpiritBody>();
		Assert.IsNotNull(_spiritBody, Utility.AssertNotNullMessage(nameof(_spiritBody)));

		_spawnPos = transform.position;
		_spiritState = SpiritState.Spawning;

		if (_tutorialSpirit)
		{
			StartCoroutine(TutorialSpiritSpawnCoroutine());
		}
		else
		{
			AudioController.PlayAudio(_audioSource, _spawnAudio);
			StartCoroutine(SpawnCoroutine());
		}
	}

	//  Update is called once per frame
	private void Update()
	{
		switch (_spiritState)
		{
			case SpiritState.StartingPossession:
				_possessedPossessable.WhileCompletingPossession(this);
				if (_possessedPossessable.PossessionCompleted)
				{
					_possessedPossessable.OnPossessionCompleted(this);
					_spiritState = SpiritState.Possessing;
					AudioController.PlayAudio(_audioSource, _completePossessionAudio);
					_moveDirection = (_spawnPos - transform.position).normalized;
				}

				break;
			case SpiritState.Possessing:
				if (_possessedPossessable is Plant plant)
				{
					plant.transform.position = transform.position;
					Move(_possessedSpeedFactor);
				}
				break;
			default:
				break;
		}
	}

	private void Move(float speedFactor = 1.0f)
	{
		_moveDirection.y = 0;
		_moveDirection = _moveDirection.normalized;

		// Use sin wave to calculate oscillating vector perpendicular to movement direction
		var perpMoveDir = 0.5f * Mathf.Sin(Time.time * 2) * (Quaternion.Euler(0, 90, 0) * _moveDirection).normalized;
		var moveDir = (_moveDirection + perpMoveDir).normalized;

		transform.rotation = transform.rotation.RotateTowards(transform.position, transform.position + moveDir, _turnSpeed * Time.deltaTime);
		transform.position += _moveSpeed * _moveSpeedMultiplier * speedFactor * Time.deltaTime * moveDir;

		// Use cos wave to make spirit bob up and down as it moves
		var bobAmount = 0.5f * Time.deltaTime * Mathf.Cos(Time.time * 3.5f);
		transform.position += Vector3.up * bobAmount;
	}

	private void DeactivateBody()
	{
		_bodyObj.SetActive(false);
	}

	private IEnumerator SpawnCoroutine()
	{
		// Set move direction towards origin with random roation applied
		_moveDirection = Quaternion.Euler(new Vector3(0, Random.Range(-60, 60), 0)) * transform.position.normalized * -1;
		float t = 0;
		while (t < _spawnMovementDelay)
		{
			Move(_justSpawnedMovementFactor);
			t += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}

		_spiritState = SpiritState.Searching;
		StartCoroutine(SearchCoroutine());
	}

	private IEnumerator TutorialSpiritSpawnCoroutine()
	{
		yield return new WaitForSeconds(_tutorialSpiritWaitDuration);
		_spiritState = SpiritState.Searching;
		_worldEvents.OnSpiritSpawned(this);
	}

	private IEnumerator SearchCoroutine()
	{
		float searchDuration = 0;
		while (_spiritState == SpiritState.Searching)
		{
			// Capture elapsed time at start of loop execution
			float time = Time.time;
			// Add possessable layers based on how long the spirit has been searching for something to possess
			int layerMask = Utility.LayersAsLayerMask(CommonTypes.Layers.Plant);
			if (searchDuration > _widenSearchThreshold)
			{
				layerMask = Utility.LayersAsLayerMask(new[] { CommonTypes.Layers.Plant, CommonTypes.Layers.SpiritWall });
			}

			// If target is not valid carry out searching behaviour to find vaild possession target
			if (_targetPossessable == null || !_targetPossessable.CanBePossessed)
			{
				// For 2 seconds rotate spirit and use raycast to search for valid possessable                
				float t = 0;
				HashSet<IPossessable> hitPossessables = new HashSet<IPossessable>();
				while (t < 2)
				{
					if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out RaycastHit hit, 100f, layerMask, QueryTriggerInteraction.Collide)
						&& hit.collider.TryGetComponent(out IPossessable hitPossessable)
						&& hitPossessable.CanBePossessed)
					{
						Debug.DrawRay(transform.position, (transform.rotation * Vector3.forward).normalized * hit.distance, Color.yellow);
						hitPossessables.Add(hitPossessable);
					}

					var bounce = Mathf.Sin(Time.time * 2f) * 0.8f;
					transform.rotation = transform.rotation.RotateTowards(transform.position,
						transform.position + transform.right * bounce,
						Time.deltaTime * _turnSpeed / 8);
					t += Time.deltaTime;
					yield return new WaitForFixedUpdate();
				}

				// Spirits target closest possessable found
				_targetPossessable = hitPossessables.OrderBy(p => Vector3.Distance(p.Transform.position, transform.position)).FirstOrDefault();
				// For three seconds if vaild possessable has not been found, move towards origin
				_moveDirection = Quaternion.Euler(new Vector3(0, Random.Range(-60, 60), 0)) * transform.position.normalized * -1;
				t = 0;
				while (t < 3 && (_targetPossessable == null || !_targetPossessable.CanBePossessed))
				{
					Move();
					t += Time.deltaTime;
					yield return new WaitForFixedUpdate();
				}
			}
			else
			{
				_moveDirection = (_targetPossessable.Transform.position - transform.position).normalized;
				Move();
			}

			// Use captured time to increment search duration
			// There is probably a better way to do this
			searchDuration += Time.time - time;
			yield return new WaitForFixedUpdate();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.IsLayer(CommonTypes.Layers.Forest) && IsPossessingPlant)
		{
			Debug.Log("Stealing a plant - collider");
			StealPossessedPlant();
		}
		else if (_spiritState != SpiritState.Searching)
		{
			// Function contents should only be carried out if spirit is searching
			return;
		}

		// Handle tutorial spirit
		if (_tutorialSpirit
			&& other.gameObject.IsLayer(CommonTypes.Layers.Plant)
			&& other.TryGetComponent(out Plant plant)
			&& plant.CanBePossessed
			&& _spiritState == SpiritState.Searching)
		{
			_possessedPossessable = plant;
			_possessedPossessable.OnPossessionStarted(this);
			transform.position = new Vector3(_possessedPossessable.Transform.position.x, transform.position.y, _possessedPossessable.Transform.position.z);
			_spiritState = SpiritState.StartingPossession;
			AudioController.PlayAudio(_audioSource, _beginPossessingAudio);
			_spiritBody.SetMaterial(_banishableMaterial);
		}

		// Handle trick plants
		if (other.gameObject.IsLayer(CommonTypes.Layers.TrickPlant)
			&& other.TryGetComponent(out TrickPlant trickPlant)
			&& trickPlant.CanBePossessed
			&& _spiritState == SpiritState.Searching)
		{
			StopAllCoroutines();
			_possessedPossessable = trickPlant;
			_possessedPossessable.OnPossessionStarted(this);
			_spiritState = SpiritState.Trapped;
			transform.position = new Vector3(_possessedPossessable.Transform.position.x, transform.position.y, _possessedPossessable.Transform.position.z);
			_spiritState = SpiritState.StartingPossession;
			_spiritBody.SetMaterial(_banishableMaterial);
		}

		//	TODO: Handle all layers for possession
		if (other.gameObject.IsInLayers(new[] { CommonTypes.Layers.Plant, CommonTypes.Layers.SpiritWall })
			&& other.TryGetComponent(out IPossessable possessable)
			&& possessable.CanBePossessed
			&& possessable == _targetPossessable
			&& _spiritState == SpiritState.Searching)
		{
			Debug.Log("Spirit start possessing");
			// Stop searching coroutine
			StopAllCoroutines();
			_possessedPossessable = possessable;
			_possessedPossessable.OnPossessionStarted(this);
			transform.position = new Vector3(_possessedPossessable.Transform.position.x, transform.position.y, _possessedPossessable.Transform.position.z);
			_spiritState = SpiritState.StartingPossession;
			AudioController.PlayAudio(_audioSource, _beginPossessingAudio);

			if (possessable is SpiritWall)
			{
				DeactivateBody();
				_worldEvents.OnSpiritWallSpawned(this);
			}
			else
			{
				_spiritBody.SetMaterial(_banishableMaterial);
			}
		}
	}
}
