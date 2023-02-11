using System;
using System.Linq;
using UnityEngine;

[Obsolete("This implementation will not be retained")]
public class LeafBlower : MonoBehaviour, IPickUp, IInteractable
{
	[SerializeField]
	private float _cooldownDuration;

	private float _cooldownProgress;

	[SerializeField]
	private float _range;

	public bool CanBeDropped => true;

	public bool CanBePickedUp => true;

	public void OnDrop()
	{
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}

	public void OnPickUp(Transform _) { }

	public GameObject PickUpObject()
	{
		return gameObject;
	}

	public bool IsInteractable => _cooldownProgress <= 0;

	public void OnPlayerInteract(PlayerInteractionController player)
	{
		//	TODO: Collider / Deprecatd
		var spirits = Physics.OverlapSphere(transform.position, _range).
			Where(s => s.GetComponent<Spirit>() != null && s.GetComponent<Spirit>().CanBeRepelled).
			Select(s => s.GetComponent<Spirit>()).
			ToList();

		foreach (var spirit in spirits)
		{
			spirit.Repel(transform.position);
		}

		_cooldownProgress = _cooldownDuration;
	}

	// Update is called once per frame
	private void Update()
	{
		if (_cooldownProgress > 0)
		{
			_cooldownProgress -= Time.deltaTime;
		}
	}
}
