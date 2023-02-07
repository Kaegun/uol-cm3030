using System.Linq;
using UnityEngine;

public class PesticideSpray : PickUpBase, ICombinable
{
	[SerializeField]
	private GameObject _contents;

	[SerializeField]
	private bool _full;

	//	TODO: Won't need this
	[SerializeField]
	private float _actionRadius = 2f;

	public void UseSpray()
	{
		_full = false;
		_contents.SetActive(false);
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
	}

	// Update is called once per frame
	private void Update()
	{
		if (_held && _full)
		{
			//	TODO: Use trigger collider
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
