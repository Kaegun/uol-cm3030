using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

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
		SpiritSpawned,
		PlantPossessed,
		SpiritWallPossessed,
		SpiritStolen,
		FireLow,
		FireDied,
		CauldronLow
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
	private Canvas _speechCanvas;

	[SerializeField]
	private float _defaultAlertDuration = 0.75f;

	[SerializeField]
	private float _defaultInstructionDuration = 3.0f;

	[SerializeField]
	private float _speechBubbleTimeout = 3.0f;

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
	private TMP_Text _speechText;
	private Camera _camera;

	private FoxState _state;
	private float _currentSpeed = 0.0f;
	private readonly Queue<Tuple<Vector3, string>> _alerts = new Queue<Tuple<Vector3, string>>();
	private readonly Queue<IEnumerator> _behaviourQueue = new Queue<IEnumerator>();
	private readonly HashSet<Events> _handledEvents = new HashSet<Events>();
	private Coroutine _activeBehaviourCoroutine;

	private readonly float _idleFollowDistance = 2.0f;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		Assert.IsNotNull(_speechCanvas, Utility.AssertNotNullMessage(nameof(_speechCanvas)));
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

		_speechText = _speechCanvas.GetComponentInChildren<TMP_Text>();
		Assert.IsNotNull(_speechText, Utility.AssertNotNullMessage(nameof(_speechText)));
		//	Hide the speech bubble on start up
		_speechCanvas.gameObject.SetActive(false);

		_camera = Camera.main;

		_worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritWallSpawned += SpiritWallSpawned;
		_worldEvents.SpiritBanished += SpiritBanished;

		_worldEvents.PlantPossessing += PlantPossessing;
		_worldEvents.PlantPossessed += PlantPossessed;
		_worldEvents.PlantStolen += PlantStolen;

		_worldEvents.FireDied += FireDied;
		_worldEvents.FireLowWarning += FireLowWarning;
		_worldEvents.FireMediumWarning += FireMediumWarning;

		_worldEvents.IngredientsLowWarning += IngredientsLowWarning;
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
			if (!Mathf.Approximately(Vector3.Dot(transform.position, _player.forward), 0))
			{
				//transform.rotation = transform.rotation.RotateTowards(transform.position, _player.forward, _turnSpeed * Time.deltaTime);

				//	TODO: Set turning anim.
			}
		}

		//switch (_state)
		//{
		//	case FoxState.Idle:
		//		//	Align fox's view to player's
		//		if (!Mathf.Approximately(Vector3.Dot(transform.position, _player.forward), 0))
		//		{
		//			transform.rotation = transform.rotation.RotateTowards(transform.position, _player.forward, _turnSpeed * Time.deltaTime);
		//
		//			//	TODO: Set turning anim.
		//		}
		//
		//		_state = (_player.position - transform.position).magnitude > _idleFollowDistance ? FoxState.Follow : FoxState.Idle;
		//		break;
		//
		//	case FoxState.Follow:
		//		//	Move the fox towards the player, whenever no other state is active
		//		Move(_player.position);
		//		break;
		//	case FoxState.Alert:
		//		if (_alerts.Count == 0)
		//		{
		//			Debug.LogWarning("Fox: Alerting with nothing to be alert about!");
		//			SetIdle();
		//			break;
		//		}
		//		var currentAlert = _alerts.Peek();
		//		Debug.Log($"Fox is alert! {currentAlert.Item1} - {currentAlert.Item2}");
		//		Move(currentAlert.Item1);
		//		if ((currentAlert.Item1 - transform.position).magnitude < _idleFollowDistance)
		//		{
		//			StartCoroutine(SpeechTextCoroutine(currentAlert.Item2));
		//			SetIdle();
		//			_alerts.Dequeue();
		//		}
		//		break;
		//}

		_animator.SetFloat(CommonTypes.AnimatorActions.ForwardSpeed, _currentSpeed);

		//	If speech bubble is active, rotate it to face the camera
		if (_speechCanvas.isActiveAndEnabled)
		{
			_speechCanvas.transform.LookAt(_camera.transform.position.ZeroY());
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

	//private IEnumerator AlertCoroutine(float duration)
	//{
	//	AudioController.PlayAudio(_audioSource, _alertSound);
	//	// Activate alert icon
	//	_worldEvents.OnFoxAlert(gameObject);
	//	_speechText.text = "!";
	//	_speechCanvas.gameObject.SetActive(true);
	//	yield return new WaitForSeconds(duration);
	//	// Disable alert icon
	//	_speechCanvas.gameObject.SetActive(false);

	//	BehaviourCoroutineCompleted();
	//}

	// Rotation not working
	private IEnumerator AlertCoroutine(float duration, Transform target)
	{
		return AlertCoroutine(duration, target.position);
		//AudioController.PlayAudio(_audioSource, _alertSound);
		//// Activate alert icon
		//_worldEvents.OnFoxAlert(gameObject);
		//_speechText.text = "!";
		//_speechCanvas.gameObject.SetActive(true);
		//float t = 0f;
		//while (t < duration)
		//{
		//	transform.rotation.RotateTowards(transform.position, target.position, _turnSpeed * Time.deltaTime);
		//	t += Time.deltaTime;
		//	yield return new WaitForEndOfFrame();
		//}
		//// Disable alert icon
		//_speechCanvas.gameObject.SetActive(false);

		//BehaviourCoroutineCompleted();
	}

	// Rotation not working
	private IEnumerator AlertCoroutine(float duration, Vector3 target)
	{
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate alert icon
		_worldEvents.OnFoxAlert(gameObject);
		_speechText.text = "!";
		_speechCanvas.gameObject.SetActive(true);
		float t = 0f;
		while (t < duration)
		{
			transform.rotation.RotateTowards(transform.position, target, _turnSpeed * Time.deltaTime);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		// Disable alert icon
		_speechCanvas.gameObject.SetActive(false);

		BehaviourCoroutineCompleted();
	}

	private IEnumerator InstructionCoroutine(string text, float duration)
	{
		// TODO: Different sound than alert for instruction?
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate instruction icon
		_speechText.text = text;
		_speechCanvas.gameObject.SetActive(true);
		yield return new WaitForSeconds(duration);
		// Disable instruction icon
		_speechCanvas.gameObject.SetActive(false);
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

	private void SetAlert(Vector3 alertPosition, string alertText)
	{
		_state = FoxState.Alert;
		_alerts.Enqueue(new Tuple<Vector3, string>(alertPosition, alertText));

		//	Play a sound
		AudioController.PlayAudio(_audioSource, _alertSound);
	}

	// Covered by SpiritSpawned
	private void SpiritWaveSpawned(object sender, Spirit[] e)
	{
		//	The fox might not do much here
		//	Could also use the camera for some of it
		Debug.Log($"Fox Behaviour: Spirit Wave Spawned - [{e.Length}]");

		//SetAlert(e[0].gameObject.transform.position, $"{(e.Length > 0 ? "Spirits have" : "A Spirit has")} spawned! They'll try steal your plants!");
	}

	private void SpiritSpawned(object sender, Spirit e)
	{
		//SetAlert(e.transform.position, $"A spirit has spawned! It'll try to steal your plants!");

		if (!_handledEvents.Contains(Events.SpiritSpawned))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine("A spirit has spawned! It'll try to steal your plants!", _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritSpawned);
		}
	}

	private void SpiritBanished(object sender, Spirit e)
	{
		Debug.Log("Fox Behaviour: Spirit was banished!");
	}

	private void SpiritWallSpawned(object sender, Spirit e)
	{
		Debug.Log("Fox Behaviour: Spirit wall spawned!");
		//SetAlert(e.gameObject.transform.position, "A spirit wall has formed. You can banish it with the Flask on the Table!");
		if (!_handledEvents.Contains(Events.SpiritWallPossessed))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine("A spirit wall has formed!", _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritWallPossessed);
		}
	}

	private void PlantPossessing(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		Debug.Log($"Fox Behaviour: Plant Possessing - [{e}]");
		//SetAlert(e, "A spirit is possessing your plant. You can banish it with the Flask on the Table!");
		//SetAlert(_alchemyTable.position, "Use the Flask on the Table to banish a spirit.");
		//SetAlert(_cauldron.position, "Remember to fill the Flask from the cauldron before you can banish a spirit.");

		if (!_handledEvents.Contains(Events.PlantPossessed))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine("A spirit is possessing one of your plants!", _defaultInstructionDuration));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
			_behaviourQueue.Enqueue(InstructionCoroutine("Fill a flask at the cauldron and use to to banish the spirit!", _defaultInstructionDuration));
			_handledEvents.Add(Events.PlantPossessed);
		}
	}

	private void PlantPossessed(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		//	More insistent
		//	Run to the bug sprayer
		Debug.Log($"Fox Behaviour: Plant Possessed - [{e}]");
	}

	private void PlantStolen(object sender, GameObject e)
	{
		Debug.Log("Fox Behaviour: A plant has been stolen!");

		//SetAlert(e, "Oh no! A spirit has stolen your plant! Don't let them steal them all!");
		if (!_handledEvents.Contains(Events.SpiritStolen))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, e.transform.position));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
			_behaviourQueue.Enqueue(InstructionCoroutine("Oh no! A spirit has stolen one of your plants! If they steal all of them you'll lose!", _defaultInstructionDuration));
			_handledEvents.Add(Events.SpiritStolen);
		}
	}

	private void FireMediumWarning(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The fire is at Medium.");

		//SetAlert(e, "The cauldron's fire is starting to run low. Add another log from the wood pile.");
		//SetAlert(_logs.position, "Fetch another log from the wood pile and add take it to the cauldron.");
		if (!_handledEvents.Contains(Events.FireLow))
		{
			_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
			_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
			_behaviourQueue.Enqueue(InstructionCoroutine("The cauldron's fire is starting to run low!", _defaultInstructionDuration));
			_handledEvents.Add(Events.FireLow);
		}
	}

	// I think just one warning at medium is enough
	private void FireLowWarning(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The fire is LOW!!");

		//SetAlert(e, "The cauldron's fire is critically low. When the fire dies, you can't make potions to banish the spirits.");
		//SetAlert(_logs.position, "Fetch another log from the wood pile and add take it to the cauldron.");
	}

	private void FireDied(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The fire has DIED!!!");

		//SetAlert(e, "The cauldron's fire has died. You won't be able to make potions to banish the spirits.");
		//SetAlert(_logs.position, "Fetch another log from the wood pile and add take it to the cauldron.");

		_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
		_behaviourQueue.Enqueue(InstructionCoroutine("The fire has died!", _defaultInstructionDuration));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_logs));
		_behaviourQueue.Enqueue(InstructionCoroutine("Take a log to the cauldron to reignite the fire!", _defaultInstructionDuration));

	}

	private void IngredientsLowWarning(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The ingredients are low!");

		//SetAlert(e, "The cauldron's ingredients are running low. You won't be able to make potions to banish the spirits.");
		//SetAlert(_alchemyTable.position, "Fetch another herb from the table and add take it to the cauldron.");
		//SetAlert(_cauldron.position, "Add the herb to the cauldron to refill your ingredient stock.");

		_behaviourQueue.Enqueue(AlertCoroutine(_defaultAlertDuration, _cauldron));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
		_behaviourQueue.Enqueue(InstructionCoroutine("The potion is running out!", _defaultInstructionDuration));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_alchemyTable));
		_behaviourQueue.Enqueue(InstructionCoroutine("Fetch a herb and take it to the cauldron to refill the potion!", _defaultInstructionDuration));
	}

	private IEnumerator SpeechTextCoroutine(string text)
	{
		//	Use a CoRoutine to despawn after a period.
		_worldEvents.OnFoxAlert(gameObject);
		_speechText.text = text;
		_speechCanvas.gameObject.SetActive(true);
		yield return new WaitForSeconds(_speechBubbleTimeout);
		_speechCanvas.gameObject.SetActive(false);
		_worldEvents.OnFoxAlertEnded(gameObject);
	}
}
