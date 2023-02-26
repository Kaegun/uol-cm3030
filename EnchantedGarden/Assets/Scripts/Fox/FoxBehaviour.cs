using System.Collections;
using System.Collections.Generic;
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

	[SerializeField]
	private Sprite _moveControlsSprite;

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
	private Image _instructionImage;
	private Camera _camera;

	private FoxState _state;
	private float _currentSpeed = 0.0f;
	private readonly Queue<IEnumerator> _behaviourQueue = new Queue<IEnumerator>();
	private readonly HashSet<Events> _handledEvents = new HashSet<Events>();
	private Coroutine _activeBehaviourCoroutine;

	private void SubscribeToWorldEvents()
	{
		_worldEvents.LevelStarted += LevelStarted;

		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritWallSpawned += SpiritWallSpawned;

		_worldEvents.PlantPossessing += PlantPossessing;
		_worldEvents.PlantStolen += PlantStolen;
		_worldEvents.PlantDroppedOutOfPatch += PlantDroppedOutOfPatch;

		_worldEvents.FireDied += FireDied;
		_worldEvents.IngredientsEmpty += IngredientsEmpty;

		_worldEvents.PickUpTrickPlant += PickUpTrickPlant;
	}

	private void UnsubscribeFromWorldEvents()
	{
		_worldEvents.LevelStarted -= LevelStarted;

		_worldEvents.SpiritSpawned -= SpiritSpawned;
		_worldEvents.SpiritWallSpawned -= SpiritWallSpawned;

		_worldEvents.PlantPossessing -= PlantPossessing;
		_worldEvents.PlantStolen -= PlantStolen;
		_worldEvents.PlantDroppedOutOfPatch -= PlantDroppedOutOfPatch;

		_worldEvents.FireDied -= FireDied;
		_worldEvents.IngredientsEmpty -= IngredientsEmpty;

		_worldEvents.PickUpTrickPlant -= PickUpTrickPlant;
	}

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

		_instructionImage = _instructionCanvas.GetComponentInChildren<Image>();
		Assert.IsNotNull(_instructionImage, Utility.AssertNotNullMessage(nameof(_instructionImage)));

		// Hide instruction canvas
		_instructionCanvas.gameObject.SetActive(false);
		_camera = Camera.main;

		SubscribeToWorldEvents();
		GameManager.Instance.CheckIngredientsEmpty();
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
		}

		_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _currentSpeed);

		//	If speech bubble is active, rotate it to face the camera
		if (_instructionCanvas.isActiveAndEnabled)
		{
			var rotation = _camera.transform.rotation.eulerAngles;
			rotation.z = 0f;
			_instructionCanvas.transform.rotation = Quaternion.Euler(rotation);
		}
	}

	private void OnDestroy()
	{
		UnsubscribeFromWorldEvents();
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
		_instructionImage.sprite = _alertSprite;
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

	private IEnumerator InstructionCoroutine(Sprite instruction, float duration)
	{
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate instruction icon
		_instructionImage.sprite = instruction;
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
		_currentSpeed = 0f;

		_animator.SetTrigger(CommonTypes.AnimatorActions.Alert);
	}

	private void SetAlert()
	{
		_state = FoxState.Alert;
	}

	private void LevelStarted(object sender, string e)
	{
		Debug.Log("Fox behaviour: Level started");
		if (_respondsTo.Contains(Events.LevelStarted) && !_handledEvents.Contains(Events.LevelStarted))
		{
			//_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration));
			if (e == CommonTypes.Scenes.Level0)
			{
				_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
				_behaviourQueue.Enqueue(InstructionCoroutine(_moveControlsSprite, _defaultInstructionDuration));
				_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
				_behaviourQueue.Enqueue(InstructionCoroutine(_spiritSpawnedSprite, _defaultInstructionDuration));
				_behaviourQueue.Enqueue(InstructionCoroutine(_spiritWillStealSprite, _defaultInstructionDuration));
				
			}			
			_handledEvents.Add(Events.LevelStarted);
		}
	}

	private void SpiritSpawned(object sender, Spirit e)
	{
		if (_respondsTo.Contains(Events.SpiritSpawned) && !_handledEvents.Contains(Events.SpiritSpawned))
		{
			//_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform));
			//_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			//_behaviourQueue.Enqueue(InstructionCoroutine(_spiritSpawnedSprite, _defaultInstructionDuration));
			//_behaviourQueue.Enqueue(InstructionCoroutine(_spiritWillStealSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritSpawned);
		}
	}

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
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e));			
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_alchemyTable));
			_behaviourQueue.Enqueue(InstructionCoroutine(_fillFlaskSprite, 4f));
			_behaviourQueue.Enqueue(InstructionCoroutine(_banishSpiritSprite, _defaultInstructionDuration));
			_handledEvents.Add(Events.PlantPossessed);
		}
	}

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

	private void PlantDroppedOutOfPatch(object sender, GameObject e)
	{
		Debug.Log("Fox Behaviour: A plant has been dropped outside of a plant patch");
		if (_respondsTo.Contains(Events.PlantDroppedOutOfPatch) && !_handledEvents.Contains(Events.PlantDroppedOutOfPatch))
		{
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(e.transform));			
			_behaviourQueue.Enqueue(InstructionCoroutine(_replantPlantSprite, _defaultInstructionDuration));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_shovel));
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform.position));
			_handledEvents.Add(Events.PlantDroppedOutOfPatch);
		}
	}

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
