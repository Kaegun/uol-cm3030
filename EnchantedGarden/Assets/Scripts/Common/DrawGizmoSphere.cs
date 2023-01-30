using UnityEngine;

public class DrawGizmoSphere : MonoBehaviour
{
	[SerializeField]
	private float _gizmoRadius = 0.5f;

	[SerializeField]
	private Color _color = Color.blue;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = _color;
		Gizmos.DrawWireSphere(transform.position, _gizmoRadius);
	}
#endif
}
