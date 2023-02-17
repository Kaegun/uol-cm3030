using UnityEngine;

public class DrawGizmoSphere : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField]
	private float _gizmoRadius = 0.5f;

	[SerializeField]
	private Color _color = Color.blue;

	private void OnDrawGizmos()
	{
		Gizmos.color = _color;
		Gizmos.DrawWireSphere(transform.position, _gizmoRadius);
	}
#endif
}
