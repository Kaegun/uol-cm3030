using System.Linq;
using UnityEngine;

public class Log : PickUpBase
{
	public override void OnDrop()
	{
		//	Use trigger (can be done in Player for both Log and Herb)
		var cauldron = Physics.OverlapSphere(transform.position, 2.0f).
			Where(c => c.GetComponent<Cauldron>() != null).
			Select(c => c.GetComponent<Cauldron>()).
			ToList();

		if (cauldron.Count > 0)
		{
			cauldron[0].AddLog();
			Destroy(gameObject);
		}
		else
		{
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		}
	}
}
