using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FoxBehaviour : MonoBehaviour
{
	/*
	 * This behaviour must listen for world events, and then instruct the Fox to react to them
	 * Test 1 - When a spirit Spawns
	 * 
	 * 
	 */

	// States - Idle, Run To Position, Alert, Instruction
	private enum FoxState
	{
		Idle,
		Follow,
		Alert,
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
	private bool _behaviourCoroutineActive = false;

	private readonly float _idleFollowDistance = 2.0f;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents);
		Assert.IsNotNull(_speechCanvas);
		Assert.IsNotNull(_player);
		Assert.IsNotNull(_logs);
		Assert.IsNotNull(_alchemyTable);
		Assert.IsNotNull(_cauldron);
		Assert.IsNotNull(_shovel);
		Assert.IsNotNull(_alertSound);

		_animator = GetComponentInChildren<Animator>();
		Assert.IsNotNull(_animator);

		_audioSource = GetComponentInChildren<AudioSource>();
		Assert.IsNotNull(_audioSource);

		_speechText = _speechCanvas.GetComponentInChildren<TMP_Text>();
		Assert.IsNotNull(_speechText);
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
		if (_behaviourQueue.Count > 0 && !_behaviourCoroutineActive)
        {
			StartCoroutine(_behaviourQueue.Dequeue());
			_behaviourCoroutineActive = true;
        }
		else if (_behaviourQueue.Count <= 0 && !_behaviourCoroutineActive)
        {
			// TODO: Set fox idle behaviour
			_currentSpeed = 0f;
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

	private void OnTriggerEnter(Collider other)
	{
		//if (other.gameObject.IsLayer(CommonTypes.Layers.Player) && _state != FoxState.Alert)
		//{
		//	SetIdle();
		//}
	}

	private IEnumerator MoveToTargetCoroutine(Transform target)
    {
		while (Vector3.Distance(transform.position, target.position) > 2f)
        {
			Move(target.position);
			yield return new WaitForFixedUpdate();
        }
		FinishBehaviourCoroutine();
    }

	private IEnumerator AlertCoroutine(float duration)
    {
		AudioController.PlayAudio(_audioSource, _alertSound);
		// Activate alert icon
		yield return new WaitForSeconds(duration);
		// Diable alert icon
		FinishBehaviourCoroutine();
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
		FinishBehaviourCoroutine();
	}

	private void FinishBehaviourCoroutine()
    {
		_behaviourCoroutineActive = false;
    }

	private void Move(Vector3 targetPosition)
	{
		_currentSpeed = Mathf.Clamp(_currentSpeed + _acceleration * Time.deltaTime, 0.0f, _maxSpeed);

		//	TODO: Check whether this works?
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

	private void SetAlert(Vector3 alertPosition, string alertText)
	{
		_state = FoxState.Alert;
		_alerts.Enqueue(new Tuple<Vector3, string>(alertPosition, alertText));

		//	Play a sound
		AudioController.PlayAudio(_audioSource, _alertSound);
	}

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
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(InstructionCoroutine("A spirit has spawned! It'll try to steal your plants!", 3f));		
    }

	private void SpiritBanished(object sender, Spirit e)
	{
		Debug.Log("Fox Behaviour: Spirit was banished!");
	}

	private void SpiritWallSpawned(object sender, Spirit e)
	{
		Debug.Log("Fox Behaviour: Spirit wall spawned!");
		//SetAlert(e.gameObject.transform.position, "A spirit wall has formed. You can banish it with the Flask on the Table!");
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(InstructionCoroutine("A spirit wall has formed!", 3f));
	}

	private void PlantPossessing(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		Debug.Log($"Fox Behaviour: Plant Possessing - [{e}]");
		//SetAlert(e, "A spirit is possessing your plant. You can banish it with the Flask on the Table!");

		//SetAlert(_alchemyTable.position, "Use the Flask on the Table to banish a spirit.");

		//SetAlert(_cauldron.position, "Remember to fill the Flask from the cauldron before you can banish a spirit.");

		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(InstructionCoroutine("A spirit is possessing one of your plants!", 3f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(InstructionCoroutine("Fill a flask at the cauldron and use to to banish the spirit!", 3f));
	}

	private void PlantPossessed(object sender, Vector3 e)
	{
		//	Alert the player
		//	Move fox and focus the camera on the fox
		//	More insistent
		//	Run to the bug sprayer
		Debug.Log($"Fox Behaviour: Plant Possessed - [{e}]");
	}

	private void PlantStolen(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: A plant has been stolen!");

		//SetAlert(e, "Oh no! A spirit has stolen your plant! Don't let them steal them all!");
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_player));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(InstructionCoroutine("Oh no! A spirit has stolen one of your plants! If they steal all of them you'll lose!", 3f));
	}

	private void FireMediumWarning(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The fire is at Medium.");

		//SetAlert(e, "The cauldron's fire is starting to run low. Add another log from the wood pile.");

		//SetAlert(_logs.position, "Fetch another log from the wood pile and add take it to the cauldron.");

		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));		
		_behaviourQueue.Enqueue(InstructionCoroutine("The cauldron's fire is starting to run low!", 3f));
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

		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_logs));
		_behaviourQueue.Enqueue(InstructionCoroutine("The fire has died! Take a log to the cauldron to reignite the fire!", 3f));
	}

	private void IngredientsLowWarning(object sender, Vector3 e)
	{
		Debug.Log("Fox Behaviour: The ingredients are low!");

		//SetAlert(e, "The cauldron's ingredients are running low. You won't be able to make potions to banish the spirits.");

		//SetAlert(_alchemyTable.position, "Fetch another herb from the table and add take it to the cauldron.");

		//SetAlert(_cauldron.position, "Add the herb to the cauldron to refill your ingredient stock.");

		_behaviourQueue.Enqueue(AlertCoroutine(1.0f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_cauldron));
		_behaviourQueue.Enqueue(InstructionCoroutine("The potion is running out!", 3f));
		_behaviourQueue.Enqueue(MoveToTargetCoroutine(_alchemyTable));
		_behaviourQueue.Enqueue(InstructionCoroutine("Fetch a herb and take it to the cauldron to refill the potion!", 3f));
	}

	private IEnumerator SpeechTextCoroutine(string text)
	{
		//	Use a CoRoutine to despawn after a period.
		_speechText.text = text;
		_speechCanvas.gameObject.SetActive(true);
		yield return new WaitForSeconds(_speechBubbleTimeout);
		_speechCanvas.gameObject.SetActive(false);
	}
}
