using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Spirit : MonoBehaviour, IInteractable
{
	enum SpiritState
	{
		Searching,
		StartingPossession,
		Possessing,
		Trapped,
		Repelled
	}

	[SerializeField]
	private float _moveSpeed;

	[SerializeField]
	private float _turnSpeed = 30f;

	[SerializeField]
	private float _repelDuration;

	private Vector3 _moveDirection;
	private float _repelProgress,
		_moveTime = 0;

	private SpiritState _spiritState;
	private Plant _possessedPlant;
	private Renderer _renderer;

	public bool CanBeBanished()
	{
		return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
	}

	public void Banish()
	{
		if (_possessedPlant != null)
		{
			_possessedPlant.Dispossess();
		}
		GameManager.Instance.ScorePoints(50);
		Destroy(gameObject);
	}

	public bool CanBeRepelled()
	{
		return _spiritState == SpiritState.Searching || _spiritState == SpiritState.Repelled;
	}

	public void Repel(Vector3 from)
	{
		_spiritState = SpiritState.Repelled;
		_repelProgress = 0;
		var direction = transform.position - from;
		direction.y = 0;
		_moveDirection = direction.normalized;
	}

	public bool IsInteractable => _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;

	public void OnPlayerInteract(PlayerInteractionController player)
	{
		Banish();
	}

	//  Start is called before the first frame update
	private void Start()
	{
		//	TODO: Set movement direction on spawn
		_moveDirection = transform.position.normalized * -1;
		_renderer = GetComponent<Renderer>();
	}

	//  Update is called once per frame
	private void Update()
	{
		switch (_spiritState)
		{
			case SpiritState.Searching:
				//  random movement for testing purposes
				_moveTime += Time.deltaTime;
				if (_moveTime >= Random.Range(2.5f, 4f))
				{
					_moveDirection = Vector3.Distance(transform.position, Vector3.zero) > 20
						? transform.position.normalized * -1
						: new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

					_moveDirection.y = 0;
					_moveDirection = _moveDirection.normalized;
					_moveTime = 0;
				}

				transform.rotation = transform.rotation.RotateTowards(transform.position, _moveDirection, _turnSpeed * Time.deltaTime);
				transform.position += _moveSpeed * Time.deltaTime * _moveDirection;

				break;
			case SpiritState.StartingPossession:
				if (_possessedPlant.PossessionThresholdReached())
				{
					_possessedPlant.CompletePossession();
					_spiritState = SpiritState.Possessing;
					//  use current position to approximate direction fastest to edge of forest
					_moveDirection = transform.position.normalized;
				}
				break;
			case SpiritState.Possessing:
				transform.position += _moveSpeed * Time.deltaTime * _moveDirection;
				_possessedPlant.transform.position = transform.position;
				break;
			case SpiritState.Repelled:
				_repelProgress += Time.deltaTime;
				if (_repelProgress > _repelDuration)
				{
					_spiritState = SpiritState.Searching;
				}
				transform.position += _moveSpeed * 2.5f * Time.deltaTime * _moveDirection;
				break;
			default:
				break;
		}
	}

	private bool IsPossessingPlant => _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;

	private void StealPossessedPlant()
	{
		GameManager.Instance.ScorePoints(-100);
		Destroy(_possessedPlant.gameObject);
		Destroy(gameObject);
	}

	private void Move()
	{
		//	TODO: Place common movement code here
	}

	private void DeactivateBody()
	{
		_renderer.enabled = false;
	}

	private void ActivateBody()
	{
		_renderer.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log($"Spirt.OnTriggerEnter: {other.gameObject.name}");
		//	TODO: I suppose this can now be a switch statement
		if (other.gameObject.IsLayer(CommonTypes.Layers.Plant))
		{
			//  handle normal plants
			_possessedPlant = other.GetComponent<Plant>();
			Assert.IsNotNull(_possessedPlant);
			_possessedPlant.StartPossession();
			transform.position = _possessedPlant.transform.position;
			_spiritState = SpiritState.StartingPossession;
			DeactivateBody();

		}
		//  handle trick plants
		else if (other.gameObject.IsLayer(CommonTypes.Layers.TrickPlant))
		{
			var trickPlant = other.GetComponent<TrickPlant>();
			Assert.IsNotNull(trickPlant);
			trickPlant.TrapSpirit(this);
			DeactivateBody();
			_spiritState = SpiritState.Trapped;
			transform.position = trickPlant.transform.position;
		}
		else if (other.gameObject.IsLayer(CommonTypes.Layers.Forest) && IsPossessingPlant)
		{
			StealPossessedPlant();
		}
	}
}
