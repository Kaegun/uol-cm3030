﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class FoxBehaviour : MonoBehaviour
{
	//	This behaviour must listen for world events, and then instruct the Fox to react to them
	private enum FoxState
	{
		Idle,
		Follow,
		Alert,
	}

	private enum Events
	{
		LevelStarted,
		SpiritSpawned,
		PlantPossessed,
		PlantDroppedOutOfPatch,
		FireDied,
		IngredientsEmpty,
		PlantStolen,
		SpiritWallSpawned,
		TrickPlantPickedUp
	}

	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Movement")]
	[SerializeField]
	private float _turnSpeed = 20.0f;

	[SerializeField]
	private float _maxSpeed = 6.0f;

	[SerializeField]
	private float _acceleration = 6.0f;

	[Header("Alert")]
	[SerializeField]
	private Canvas _instructionCanvas;

	[SerializeField]
	private float _defaultAlertDuration = 0.75f;

	[SerializeField]
	private float _defaultInstructionDuration = 3.0f;

	[SerializeField]
	private List<Events> _respondsTo = new List<Events> {
		Events.LevelStarted,
		Events.SpiritSpawned,
		Events.PlantPossessed,
		Events.PlantDroppedOutOfPatch,
		Events.FireDied,
		Events.IngredientsEmpty,
		Events.PlantStolen,
		Events.SpiritWallSpawned,
		Events.TrickPlantPickedUp
	};

	[Header("Instruction Sprites")]
	[SerializeField]
	private Sprite _alertSprite;

	[SerializeField]
	private Sprite _spiritSpawnedSprite;

	[SerializeField]
	private Sprite _spiritWillStealSprite;

	[SerializeField]
	private Sprite _fillFlaskSprite;

	[SerializeField]
	private Sprite _banishSpiritSprite;

	[SerializeField]
	private Sprite _replantPlantSprite;

	[SerializeField]
	private Sprite _cauldronUnusableSprite;

	[SerializeField]
	private Sprite _refillPotionSprite;

	[SerializeField]
	private Sprite _refuelFireSprite;

	[SerializeField]
	private Sprite _lostPlantSprite;

	[SerializeField]
	private Sprite _zeroPlantsIsLoseSprite;

	[SerializeField]
	private Sprite _trickPlantTrapSprite;

	[SerializeField]
	private Sprite _spiritWallNoEntrySprite;

	[SerializeField]
	private Sprite _spritWallBanishSprite;

	//[SerializeField]
	//private float _speechBubbleTimeout = 3.0f;

	[Header("Points of Interest")]
	[SerializeField]
	private Transform _player;

	[SerializeField]
	private Transform _logs;

	[SerializeField]
	private Transform _alchemyTable;

	[SerializeField]
	private Transform _cauldron;

	[SerializeField]
	private Transform _shovel;


	[Header("Audio")]
	[SerializeField]
	private ScriptableAudioClip _alertSound;

	private Animator _animator;
	private AudioSource _audioSource;
	//private TMP_Text _speechText;
	private Image _inscructionImage;
	private Camera _camera;

	private FoxState _state;
	private float _currentSpeed = 0.0f;
	private readonly Queue<IEnumerator> _behaviourQueue = new Queue<IEnumerator>();
	private readonly HashSet<Events> _handledEvents = new HashSet<Events>();
	private Coroutine _activeBehaviourCoroutine;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		Assert.IsNotNull(_instructionCanvas, Utility.AssertNotNullMessage(nameof(_instructionCanvas)));
		Assert.IsNotNull(_player, Utility.AssertNotNullMessage(nameof(_player)));
		Assert.IsNotNull(_logs, Utility.AssertNotNullMessage(nameof(_logs)));
		Assert.IsNotNull(_alchemyTable, Utility.AssertNotNullMessage(nameof(_alchemyTable)));
		Assert.IsNotNull(_cauldron, Utility.AssertNotNullMessage(nameof(_cauldron)));
		Assert.IsNotNull(_shovel, Utility.AssertNotNullMessage(nameof(_shovel)));
		Assert.IsNotNull(_alertSound, Utility.AssertNotNullMessage(nameof(_alertSound)));

		_animator = GetComponentInChildren<Animator>();
		Assert.IsNotNull(_animator, Utility.AssertNotNullMessage(nameof(_animator)));

		_audioSource = GetComponentInChildren<AudioSource>();
		Assert.IsNotNull(_audioSource, Utility.AssertNotNullMessage(nameof(_audioSource)));

		//_speechText = _instructionCanvas.GetComponentInChildren<TMP_Text>();
		//Assert.IsNotNull(_speechText, Utility.AssertNotNullMessage(nameof(_speechText)));

		_inscructionImage = _instructionCanvas.GetComponentInChildren<Image>();
		Assert.IsNotNull(_inscructionImage, Utility.AssertNotNullMessage(nameof(_inscructionImage)));

		// Hide instruction canvas
		_instructionCanvas.gameObject.SetActive(false);


		_camera = Camera.main;

		_worldEvents.LevelStarted += LevelStarted;

		//_worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritWallSpawned += SpiritWallSpawned;
		//_worldEvents.SpiritBanished += SpiritBanished;

		_worldEvents.PlantPossessing += PlantPossessing;
		//_worldEvents.PlantPossessed += PlantPossessed;
		_worldEvents.PlantStolen += PlantStolen;
		_worldEvents.PlantDroppedOutOfPatch += PlantDroppedOutOfPatch;

		_worldEvents.FireDied += FireDied;
		//_worldEvents.FireLowWarning += FireLowWarning;
		//_worldEvents.FireMediumWarning += FireMediumWarning;

		//_worldEvents.IngredientsLowWarning += IngredientsLowWarning;
		_worldEvents.IngredientsEmpty += IngredientsEmpty;

		_worldEvents.PickUpTrickPlant += PickUpTrickPlant;
	}

	// Update is called once per frame
	private void Update()
	{
		if (_behaviourQueue.Count > 0 && _activeBehaviourCoroutine == null)
		{
			_activeBehaviourCoroutine = StartCoroutine(_behaviourQueue.Dequeue());
			_worldEvents.OnFoxAlert(gameObject);
			SetAlert();
		}

		if (_state == FoxState.Idle)
		{
			//if (!Mathf.Approximately(Vector3.Dot(transform.position, _player.forward), 0))
			//{
			//	//transform.rotation = transform.rotation.RotateTowards(transform.position, _player.forward, _turnSpeed * Time.deltaTime);

			//	//	TODO: Set turning anim.
			//}
		}

		_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _currentSpeed);

		//	If speech bubble is active, rotate it to face the camera
		//	TODO: Could move this to the Update method on a script on the Canvas itself.
		if (_instructionCanvas.isActiveAndEnabled)
		{
			//_speechCanvas.transform.LookAt(_camera.transform.position.ZeroY());
			var rotation = _camera.transform.rotation.eulerAngles;
			rotation.z = 0f;
			_instructionCanvas.transform.rotation = Quaternion.Euler(rotation);
		}
	}

	private IEnumerator MoveToTargetCoroutine(Transform target)
	{
		while (Vector3.Distance(transform.position, target.position) > 4f)
		{
			Move(target.position);
			yield return new WaitForEndOfFrame();
		}
		BehaviourCoroutineCompleted();
	}

	// Rotation not working
	private IEnumerator AlertCoroutine(float duration, Transform target)
	{
		return AlertCoroutine(duration, target.position);
	}

	// Rotation not working
	private IEnumerator AlertCoroutine(float duration, Vector3? target = null)
	{
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate alert icon
		_worldEvents.OnFoxAlert(gameObject);
		_inscructionImage.sprite = _alertSprite;
		_instructionCanvas.gameObject.SetActive(true);
		float t = 0f;
		while (t < duration)
		{
			if (target != null)
            {
				transform.rotation.RotateTowards(transform.position, target.Value, _turnSpeed * Time.deltaTime);
			}			
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		// Disable alert icon
		_instructionCanvas.gameObject.SetActive(false);

		BehaviourCoroutineCompleted();
	}

	//private IEnumerator InstructionCoroutine(string text, float duration)
	//{
	//	// TODO: Different sound than alert for instruction?
	//	AudioController.PlayAudio(_audioSource, _alertSound);
	//	// Activate instruction icon
	//	_speechText.text = text;
	//	_instructionCanvas.gameObject.SetActive(true);
	//	yield return new WaitForSeconds(duration);
	//	// Disable instruction icon
	//	_instructionCanvas.gameObject.SetActive(false);
	//	BehaviourCoroutineCompleted();
	//}

	private IEnumerator InstructionCoroutine(Sprite instruction, float duration)
	{
		// TODO: Different sound than alert for instruction?
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate instruction icon
		_inscructionImage.sprite = instruction;
		_instructionCanvas.gameObject.SetActive(true);
		yield return new WaitForSeconds(duration);
		// Disable instruction icon
		_instructionCanvas.gameObject.SetActive(false);
		BehaviourCoroutineCompleted();
	}

	private void BehaviourCoroutineCompleted()
	{
		_activeBehaviourCoroutine = null;
		SetIdle();
		_worldEvents.OnFoxAlertEnded(gameObject);
	}

	private void Move(Vector3 targetPosition)
	{
		_currentSpeed = Mathf.Clamp(_currentSpeed + _acceleration * Time.deltaTime, 0.0f, _maxSpeed);

		transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, targetPosition, _currentSpeed * Time.deltaTime),
			transform.rotation.RotateTowards(transform.position, targetPosition, _turnSpeed * Time.deltaTime));
	}

	private void SetIdle()
	{
		_state = FoxState.Idle;
		//	TODO: A bit abrupt
		_currentSpeed = 0f;

		//	TODO: Test Alert animation
		_animator.SetTrigger(CommonTypes.AnimatorActions.Alert);
	}

	private void SetAlert()
	{
		_state = FoxState.Alert;
	}

	// Covered by SpiritSpawned
	//private void SpiritWaveSpawned(object sender, Spirit[] e)
	//{
	//	//	The fox might not do much here
	//	//	Could also use the camera for some of it
	//	Debug.Log($"Fox Behaviour: Spirit Wave Spawned - [{e.Length}]");
	//}

	private void LevelStarted(object sender, string e)
	{
		if (_respondsTo.Contains(Events.LevelStarted) && !_handledEvents.Contains(Events.LevelStarted))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration));
			if (e == CommonTypes.Scenes.Level0)
            {
				_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
				// Give move controls instruction
				//_behaviourQueue.Enqueue(InstructionCoroutine(_moveControlsSprite, _defaultInstructionDuration));
			}			
			_handledEvents.Add(Events.LevelStarted);
		}
	}

	private void SpiritSpawned(object sender, Spirit e)
	{
		if (_respondsTo.Contains(Events.SpiritSpawned) && !_handledEvents.Contains(Events.SpiritSpawned))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine(_spiritSpawnedSprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(InstructionCoroutine(_spiritWillStealSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritSpawned);
		}
	}

	// Unnecessary for fox to track
	//private void SpiritBanished(object sender, Spirit e)
	//{
	//	Debug.Log("Fox Behaviour: Spirit was banished!");
	//}

	private void SpiritWallSpawned(object sender, Spirit e)
	{
		Debug.Log("Fox Behaviour: Spirit wall spawned!");

		if (_respondsTo.Contains(Events.SpiritWallSpawned) && !_handledEvents.Contains(Events.SpiritWallSpawned))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine(_spiritWallNoEntrySprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(InstructionCoroutine(_spritWallBanishSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritWallSpawned);
		}
	}

	private void PlantPossessing(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		Debug.Log($"Fox Behaviour: Plant Possessing - [{e}]");

		if (_respondsTo.Contains(Events.PlantPossessed) && !_handledEvents.Contains(Events.PlantPossessed))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			// TODO: Add spirit possessing instruction
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
			_behaviourQueue.Enqueue(InstructionCoroutine(_fillFlaskSprite, 4f));
			_behaviourQueue.Enqueue(InstructionCoroutine(_banishSpiritSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.PlantPossessed);
		}
	}

	// Covered by plant possessing
	//private void PlantPossessed(object sender, Vector3 e)
	//{
	//	//	Alert the player
	//	//	Move fox and focus the camera on the fox
	//	//	More insistent
	//	//	Run to the bug sprayer
	//	Debug.Log($"Fox Behaviour: Plant Possessed - [{e}]");
	//}

	private void PlantStolen(object sender, GameObject e)
	{
		Debug.Log("Fox Behaviour: A plant has been stolen!");
		if (_respondsTo.Contains(Events.PlantStolen) && !_handledEvents.Contains(Events.PlantStolen))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform.position));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine(_lostPlantSprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(InstructionCoroutine(_zeroPlantsIsLoseSprite, 4f));
			_handledEvents.Add(Events.PlantStolen);
		}
	}

	private void PlantDroppedOutOfPatch (object sender, GameObject e)
	{
		Debug.Log("Fox Behaviour: A has been dropped outside of a plant patch");
		if (_respondsTo.Contains(Events.PlantDroppedOutOfPatch) && !_handledEvents.Contains(Events.PlantDroppedOutOfPatch))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform.position));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(e.transform));
			_behaviourQueue.Enqueue(InstructionCoroutine(_replantPlantSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.PlantDroppedOutOfPatch);
		}
	}


	// Only alert on fire died
	//private void FireMediumWarning(object sender, Vector3 e)
	//{
	//	Debug.Log("Fox Behaviour: The fire is at Medium.");
	//
	//	//SetAlert(e, "The cauldron's fire is starting to run low. Add another log from the wood pile.");
	//	//SetAlert(_logs.position, "Fetch another log from the wood pile and add take it to the cauldron.");
	//	if (!_handledEvents.Contains(Events.FireLow))
	//	{
	//		_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
	//		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
	//		//_behaviourQueue.Enqueue(InstructionCoroutine("The cauldron's fire is starting to run low!", _defaultInstructionDuration));
	//		_handledEvents.Add(Events.FireLow);
	//	}
	//}

	//// I think just one warning at medium is enough
	//private void FireLowWarning(object sender, Vector3 e)
	//{
	//	Debug.Log("Fox Behaviour: The fire is LOW!!");
	//}

	private void FireDied(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The fire has DIED!!!");
		if (_respondsTo.Contains(Events.FireDied))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
			_behaviourQueue.Enqueue(InstructionCoroutine(_cauldronUnusableSprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_logs));
			_behaviourQueue.Enqueue(InstructionCoroutine(_refuelFireSprite, _defaultInstructionDuration));
		}
	}

	// Only alert on ingredients empty
	//private void IngredientsLowWarning(object sender, Vector3 e)
	//{
	//	Debug.Log("Fox Behaviour: The ingredients are low!");
	//	if (_respondsTo.Contains(Events.SpiritSpawned))
	//	{
	//		_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
	//		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
	//		//_behaviourQueue.Enqueue(InstructionCoroutine("The potion is running out!", _defaultInstructionDuration));
	//		_behaviourQueue.Enqueue(InstructionCoroutine(_cauldronUnusableSprite, _defaultInstructionDuration));
	//		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_alchemyTable));
	//		//_behaviourQueue.Enqueue(InstructionCoroutine("Fetch a herb and take it to the cauldron to refill the potion!", _defaultInstructionDuration));
	//		_behaviourQueue.Enqueue(InstructionCoroutine(_refillPotionSprite, _defaultInstructionDuration));
	//	}
	//}

	private void IngredientsEmpty(object send, Vector3 e)
    {
		Debug.Log("Fox Behaviour: The ingredients are empty");
		if (_respondsTo.Contains(Events.IngredientsEmpty))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
			_behaviourQueue.Enqueue(InstructionCoroutine(_cauldronUnusableSprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_alchemyTable));
			_behaviourQueue.Enqueue(InstructionCoroutine(_refillPotionSprite, _defaultInstructionDuration));
		}
	}

	private void PickUpTrickPlant(object send, GameObject e)
    {
		if (_respondsTo.Contains(Events.TrickPlantPickedUp) && !_handledEvents.Contains(Events.TrickPlantPickedUp))
		{
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _player));			
			_behaviourQueue.Enqueue(InstructionCoroutine(_trickPlantTrapSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.TrickPlantPickedUp);
		}
	}
}
