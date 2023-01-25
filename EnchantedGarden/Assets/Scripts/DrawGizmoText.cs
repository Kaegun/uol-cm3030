using UnityEditor;
using UnityEngine;

//	Found here: https://gist.github.com/Arakade/9dd844c2f9c10e97e3d0
public class DrawGizmoText : MonoBehaviour
{
	[SerializeField]
	private int _textSize = 15;

	[SerializeField]
	private Color _color = Color.blue;

	[SerializeField]
	private string _text = "gizmo text";

	[SerializeField]
	private bool _showName = false;

	[SerializeField]
	private Vector2 _anchor;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var view = SceneView.currentDrawingSceneView;
		if (!view)
			return;

		Vector3 screenPosition = view.camera.WorldToScreenPoint(transform.position);
		if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
			return;

		var pixelRatio = EditorGUIUtility.pixelsPerPoint;
		Handles.BeginGUI();
		var style = new GUIStyle(GUI.skin.label)
		{
			fontSize = _textSize,
			normal = new GUIStyleState() { textColor = _color }
		};

		var text = _showName ? name : _text;
		Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
		var alignedPosition = ((Vector2)screenPosition
			+ size * ((_anchor + Vector2.left + Vector2.up) / 2f))
			* (Vector2.right + Vector2.down) +
			Vector2.up * view.camera.pixelHeight;

		GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);

		Handles.EndGUI();
	}
#endif
}
