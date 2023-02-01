using UnityEngine;


//	TODO: Remove this class
public class SpiritSpawnPoint : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1);
	}
}
