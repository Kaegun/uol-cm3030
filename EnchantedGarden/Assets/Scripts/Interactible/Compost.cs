using System.Linq;
using UnityEngine;

public class Compost : MonoBehaviour, IPickUp
{
	public bool CanBeDropped => true;

	public bool CanBePickedUp => true;

	public void OnDrop()
	{
		var plantPatches = Physics.OverlapSphere(transform.position, 2.0f).
			Where(c => c.GetComponent<PlantPatch>() != null && !c.GetComponent<PlantPatch>().ContainsCompost()).
			Select(c => c.GetComponent<PlantPatch>()).
			OrderBy(c => Vector3.Distance(c.transform.position, transform.position)).
			ToList();

		if (plantPatches.Count > 0)
		{
			//	Looks like this is hardcoded?
			plantPatches[0].AddCompost();
			Destroy(gameObject);
		}
		else
		{
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		}
	}

	public void OnPickUp() { }

	public GameObject PickUpObject()
	{
		return gameObject;
	}
}
