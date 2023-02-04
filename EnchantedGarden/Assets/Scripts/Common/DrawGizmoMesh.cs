using UnityEngine;

public class DrawGizmoMesh : MonoBehaviour
{
	[SerializeField]
	private Color _color = Color.blue;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var mesh = GetComponent<MeshFilter>().mesh;
		Gizmos.color = _color;
		Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.localScale);
	}
#endif
}
