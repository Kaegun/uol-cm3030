using System.Linq;
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
	private SpiritState _spiritState;

	[SerializeField]
	private float _moveSpeed;
	private Vector3 _moveDirection;
	private float _moveTime = 0;

	private Plant _possessedPlant;

	[SerializeField]
	private GameObject _spiritBody;

	[SerializeField]
	private float _repelDuration;
	private float _repelProgress;

	//  Start is called before the first frame update
	private void Start()
	{
		// set movement direction on spawn
		_moveDirection = transform.position.normalized * -1;
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
					if (Vector3.Distance(transform.position, Vector3.zero) > 20)
					{
						_moveDirection = transform.position.normalized * -1;
					}
					else
					{
						_moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
					}
					_moveDirection.y = 0;
					_moveDirection = _moveDirection.normalized;
					_moveTime = 0;

				}
				transform.position += _moveSpeed * Time.deltaTime * _moveDirection;

				//  check for nearby possessable plants and start to possess one
				var plants = Physics.OverlapSphere(transform.position, 2f).
					Where(c => (c.GetComponent<Plant>() != null && c.GetComponent<Plant>().CanBePossessed()) || (c.GetComponent<TrickPlant>() != null && c.GetComponent<TrickPlant>().CanTrapSpirit())).
					ToList();
				if (plants.Count > 0)
				{
					//  handle normal plants
					if (plants[0].GetComponent<Plant>() != null)
					{
						var plant = plants[0].GetComponent<Plant>();
						_possessedPlant = plant;
						_possessedPlant.StartPossession();
						transform.position = _possessedPlant.transform.position;
						_spiritState = SpiritState.StartingPossession;
						_spiritBody.SetActive(false);

					}
					//  handle trick plants
					else if (plants[0].GetComponent<TrickPlant>() != null)
					{
						var trickPlant = plants[0].GetComponent<TrickPlant>();
						trickPlant.TrapSpirit(this);
						_spiritBody.SetActive(false);
						_spiritState = SpiritState.Trapped;
						transform.position = trickPlant.transform.position;
					}
				}
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
		GameManager.instance.ScorePoints(50);
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

	public bool PossessingPlant()
	{
		return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
	}

	public void StealPossessedPlant()
	{
		GameManager.instance.ScorePoints(-100);
		Destroy(_possessedPlant.gameObject);
		Destroy(gameObject);
	}

	public bool CanBeInteractedWith()
	{
		return _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;
	}

	public void OnPlayerInteract(PlayerInteractionController player)
	{
		Banish();
	}
}
