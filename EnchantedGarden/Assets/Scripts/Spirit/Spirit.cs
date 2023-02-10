using UnityEngine;

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
	//private Renderer _renderer;

	public bool CanBeBanished => _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;

	public void Banish()
	{
		if (_possessedPlant != null)
		{
			_possessedPlant.Dispossess();
		}
		GameManager.Instance.ScorePoints(50);
		Destroy(gameObject);
	}

	public bool CanBeRepelled => _spiritState == SpiritState.Searching || _spiritState == SpiritState.Repelled;

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
		//	TODO: We may not want to do this
		//_renderer = GetComponent<Renderer>();
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

				Move();

				break;
			case SpiritState.StartingPossession:
				if (_possessedPlant.PossessionThresholdReached)
				{
					_possessedPlant.CompletePossession();
					_spiritState = SpiritState.Possessing;
					//  use current position to approximate direction fastest to edge of forest
					_moveDirection = transform.position.normalized;
				}

				break;
			case SpiritState.Possessing:
				//	TODO: Ensure all movement is using common Move method
				transform.position += _moveSpeed * Time.deltaTime * _moveDirection;
				_possessedPlant.transform.position = transform.position;
				break;
			case SpiritState.Repelled:
				_repelProgress += Time.deltaTime;
				if (_repelProgress > _repelDuration)
				{
					_spiritState = SpiritState.Searching;
				}

				Move(2.5f);

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

	private void Move(float speedFactor = 1.0f)
	{
		//	TODO: Place common movement code here        
		transform.rotation = transform.rotation.RotateTowards(transform.position, transform.position + _moveDirection, _turnSpeed * Time.deltaTime);
		transform.position += _moveSpeed * Time.deltaTime * _moveDirection * speedFactor;

	}

	private void DeactivateBody()
	{
		//	TODO: We may not want to do this
		//_renderer.enabled = false;
	}

	private void ActivateBody()
	{
		//	TODO: We may not want to do this
		//_renderer.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log($"Spirt.OnTriggerEnter: {other.gameObject.name}");
		//	Handle plants
		if (other.gameObject.IsLayer(CommonTypes.Layers.Plant)
			&& other.TryGetComponent(out _possessedPlant)
			&& _possessedPlant.CanBePossessed)
		{
			//  handle normal plants
			_possessedPlant.StartPossession();
			transform.position = _possessedPlant.transform.position;
			_spiritState = SpiritState.StartingPossession;
			DeactivateBody();

		}
		else if (other.gameObject.IsLayer(CommonTypes.Layers.TrickPlant)
			&& other.TryGetComponent(out TrickPlant trickPlant)
			&& trickPlant.CanTrapSpirit)
		{
			//  handle trick plants
			trickPlant.TrapSpirit(this);
			DeactivateBody();
			_spiritState = SpiritState.Trapped;
			transform.position = trickPlant.transform.position;
		}
		else if (other.gameObject.IsLayer(CommonTypes.Layers.Forest) && IsPossessingPlant)
		{
			//	handle - we're off the edge of the map Jim
			StealPossessedPlant();
		}
	}
}
