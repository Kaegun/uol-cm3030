using System.Linq;
using UnityEngine;

public class PesticideSpray : MonoBehaviour, IPickUp, ICombinable
{
	[SerializeField]
	private GameObject _contents;

	[SerializeField]
	private bool _full;

	[SerializeField]
	private float _actionRadius = 2f;

	private bool _held;

	public void UseSpray()
	{
		_full = false;
		_contents.SetActive(false);
	}

	public bool CanBeDropped => true;

	public bool CanBePickedUp => true;

	public void OnDrop()
	{
		_held = false;
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}

	public void OnPickUp() { }

	public GameObject PickUpObject()
	{
		_held = true;
		return gameObject;
	}

	public void OnCombine()
	{
		_full = true;
		_contents.SetActive(true);
	}

	public bool CanBeCombined()
	{
		return _held && !_full;
	}

	// Start is called before the first frame update
	private void Start()
	{
		_full = false;
		_contents.SetActive(false);
		_held = false;
	}

	// Update is called once per frame
	private void Update()
	{
		if (_held && _full)
		{
			// Check for nearby banishable spirits
			var spirits = Physics.OverlapSphere(transform.position, _actionRadius).
			Where(s => s.GetComponent<Spirit>() != null && s.GetComponent<Spirit>().CanBeBanished()).
			Select(s => s.GetComponent<Spirit>()).
			OrderBy(s => Vector3.Distance(s.transform.position, transform.position)).
			ToList();

			// If there are banishable spirits nearby, spray the closest one
			if (spirits.Count > 0)
			{
				UseSpray();
				spirits[0].Banish();
			}
		}
	}
}
