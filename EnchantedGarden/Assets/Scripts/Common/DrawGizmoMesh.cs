using UnityEngine;

public class DrawGizmoMesh : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField]
	private Color _color = Color.blue;

	private void OnDrawGizmos()
	{
		var mesh = GetComponent<MeshFilter>().sharedMesh;
		Gizmos.color = _color;
		Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.localScale);
	}
#endif
}
