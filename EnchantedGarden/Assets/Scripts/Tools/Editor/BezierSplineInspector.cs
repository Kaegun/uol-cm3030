using UnityEditor;
using UnityEngine;

//	REF: https://catlikecoding.com/unity/tutorials/curves-and-splines/
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
	private BezierSpline _spline;
	private Transform _handleTransform;
	private Quaternion _handleRotation;
	private int _selectedIndex = -1;

	private const int _stepsPerCurve = 10;
	private const float _directionScale = 0.5f,
		_handleSize = 0.04f,
		_pickSize = 0.06f;

	private void OnSceneGUI()
	{
		_spline = target as BezierSpline;
		_handleTransform = _spline.transform;
		_handleRotation = _handleTransform.HandleRotation();

		var p0 = ShowPoint(0);

		for (int i = 1; i < _spline.ControlPointCount; i += 3)
		{
			var p1 = ShowPoint(i);
			var p2 = ShowPoint(i + 1);
			var p3 = ShowPoint(i + 2);

			Handles.color = _spline.LineColour;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p1, p2);
			Handles.DrawLine(p2, p3);

			Handles.DrawBezier(p0, p3, p1, p2, GetPointColour(i), _spline.Texture, _spline.Width);
			p0 = p3;
		}

		ShowDirections();
	}

	public override void OnInspectorGUI()
	{
		_spline = target as BezierSpline;

		//	Set the spline to be a loop
		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", _spline.Loop);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(_spline, "Toggle Loop");
			EditorUtility.SetDirty(_spline);
			_spline.Loop = loop;
		}

		if (_selectedIndex >= 0 && _selectedIndex < _spline.ControlPointCount)
			DrawSelectedPointInspector();

		//	Add a curve to the spline
		EditorGUI.BeginChangeCheck();
		if (GUILayout.Button("Add Curve"))
		{
			Undo.RecordObject(_spline, "Add Curve");
			_spline.AddCurve();
			EditorUtility.SetDirty(_spline);
		}

		//	Change the spline's direction
		EditorGUI.BeginChangeCheck();
		if (GUILayout.Button("Change Direction"))
		{
			Undo.RecordObject(_spline, "Change Direction");
			_spline.ChangeDirection();
			EditorUtility.SetDirty(_spline);
		}
	}

	private void DrawSelectedPointInspector()
	{
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", _spline.GetControlPoint(_selectedIndex));

		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(_spline, "Move Point");
			EditorUtility.SetDirty(_spline);
			_spline.SetControlPoint(_selectedIndex, point);
		}

		EditorGUI.BeginChangeCheck();
		BezierSpline.ControlPointMode mode = (BezierSpline.ControlPointMode)EditorGUILayout.EnumPopup("Mode", _spline.GetControlPointMode(_selectedIndex));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(_spline, "Change Point Mode");
			_spline.SetControlPointMode(_selectedIndex, mode);
			EditorUtility.SetDirty(_spline);
		}
	}

	private void ShowDirections()
	{
		Handles.color = _spline.VelocityColour;
		Vector3 point = _spline.GetPoint(0f);
		Handles.DrawLine(point, point + _spline.GetDirection(0f) * _directionScale);
		var steps = _stepsPerCurve * _spline.CurveCount;
		for (int i = 1; i <= steps; i++)
		{
			point = _spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + _spline.GetDirection(i / (float)steps) * _directionScale);
		}
	}

	private Vector3 ShowPoint(int index)
	{
		Vector3 point = _handleTransform.TransformPoint(_spline.GetControlPoint(index));
		Handles.color = GetPointColour(index);
		Handles.Label(point + new Vector3(0.5f, 0f, 0.5f), index.ToString());
		float size = HandleUtility.GetHandleSize(point);

		if (Handles.Button(point, _handleRotation, size * _handleSize, size * _pickSize, Handles.DotHandleCap))
		{
			_selectedIndex = index;
			Repaint();
		}

		if (_selectedIndex == index)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, _handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_spline, "Move Point");
				EditorUtility.SetDirty(_spline);
				_spline.SetControlPoint(index, _handleTransform.InverseTransformPoint(point));
			}
		}

		return point;
	}

	private Color GetPointColour(int index)
	{
		return _spline.PointColours[(int)_spline.GetControlPointMode(index)];
	}
}
