using System.Linq;
using UnityEngine;

public class Shovel : PickUpBase
{
	//	TODO: Will be handled on player by animation
	[SerializeField]
	private float _actionDuration = 1f;
	private float _actionProgress;

	//	Start is called before the first frame update
	private void Start()
	{
		_actionProgress = 0;
	}

	//	Update is called once per frame
	private void Update()
	{
		if (_held)
		{
			//	TODO: This logic is better served in the PlayerController
			//var plants = Physics.OverlapSphere(transform.position, _actionRadius).
			//Where(p => p.GetComponent<Plant>() != null && p.GetComponent<Plant>().CanBeReplanted()).
			//Select(p => p.GetComponent<Plant>()).
			//OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).
			//ToList();

			//	TODO: Animate digging
			//if (plants.Count > 0)
			//{
			//	float t = Mathf.PingPong(Time.time * 3, 1);
			//	//_model.transform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 0.75f, t);
			//	_actionProgress += Time.deltaTime;
			//	if (_actionProgress > _actionDuration)
			//	{
			//		plants[0].Replant();
			//		_actionProgress = 0;
			//	}
			//}
			//else
			//{
			//	_actionProgress = 0;
			//	//_model.transform.localPosition = Vector3.zero;
			//}
		}
		else
		{
			_actionProgress = 0;
			//_model.transform.localPosition = Vector3.zero;
		}
	}
}
