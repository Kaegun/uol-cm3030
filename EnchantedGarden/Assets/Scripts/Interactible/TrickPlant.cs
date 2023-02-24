using UnityEngine;
using System.Collections;

public class TrickPlant : PickUpBase, IPossessable
{
	enum PlantState
	{
		Default,
		Possessed,
		Carried,
	}

	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[SerializeField]
	private float _growthDuration = 1.0f;

	[SerializeField]
	private float _growthTarget = 2.0f;

	[Header("Canvas")]
	[SerializeField]
	private Canvas _canvas;


	private PlantState _plantState;
	private float _growthProgress = 0;

	private bool FullyGrown => _growthProgress >= _growthDuration;	

	public override bool CanBePickedUp => _plantState == PlantState.Default;

    public bool CanBePossessed => FullyGrown && _plantState == PlantState.Default;

	// Possession cannot be completed as the spirit is stuck possessing the trick plant until banished. Therefore this will always return false
	public bool PossessionCompleted => false;

    public GameObject GameObject => gameObject;

	public void OnPossessionStarted(Spirit possessor)
	{
		// Set CanBePossessed false
		_plantState = PlantState.Possessed;
		_canvas.transform.rotation = Quaternion.Euler(-Camera.main.transform.rotation.eulerAngles);
		_canvas.gameObject.SetActive(true);
	}

	public void WhileCompletingPossession(Spirit possessor)
	{
		// Do nothing here
	}

	public void OnPossessionCompleted(Spirit possessor)
	{
		// Possession cannot be completed as the spirit is stuck possessing the trick plant until banished. Therefore do nothing here
	}

	public void OnDispossess()
	{
		_plantState = PlantState.Default;
		_canvas.gameObject.SetActive(false);
	}

	public override void OnDrop(bool despawn = false)
	{
		base.OnDrop();
		_plantState = PlantState.Default;
		transform.localScale /= _growthTarget;
		StartCoroutine(GrowOnDropCoroutine());
	}

	public override void OnPickUp(Transform pickupTransform)
	{
		_worldEvents.OnPickUpTrickPlant(gameObject);
		_plantState = PlantState.Carried;
		_growthProgress = 0f;
		base.OnPickUp(pickupTransform);		
	}

	private IEnumerator GrowOnDropCoroutine()
	{
		while (!_held && _growthProgress <= _growthDuration)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(_growthTarget, _growthTarget, _growthTarget), (_growthProgress / _growthDuration));
			_growthProgress += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	protected override void Start()
    {
		base.Start();
		_canvas.gameObject.SetActive(false);
    }
}
