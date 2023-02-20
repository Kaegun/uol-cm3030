using UnityEditor;
using UnityEngine;

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

		for (int i = 1; i < _spline.Points.Length; i += 3)
		{
			var p1 = ShowPoint(i);
			var p2 = ShowPoint(i + 1);
			var p3 = ShowPoint(i + 2);

			Handles.color = _spline.LineColour;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p1, p2);
			Handles.DrawLine(p2, p3);

			Handles.DrawBezier(p0, p3, p1, p2, _spline.CurveColour, _spline.Texture, _spline.Width);
			p0 = p3;
		}

		ShowDirections();
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		_spline = target as BezierSpline;
		if (GUILayout.Button("Add Curve"))
		{
			Undo.RecordObject(_spline, "Add Curve");
			_spline.AddCurve();
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
		Vector3 point = _handleTransform.TransformPoint(_spline.Points[index]);
		Handles.color = _spline.CurveColour;
		Handles.Label(point + new Vector3(0.5f, 0f, 0.5f), index.ToString());
		float size = HandleUtility.GetHandleSize(point);
		if (Handles.Button(point, _handleRotation, size * _handleSize, size * _pickSize, Handles.DotHandleCap))
			_selectedIndex = index;

		if (_selectedIndex == index)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, _handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_spline, "Move Point");
				EditorUtility.SetDirty(_spline);
				_spline.Points[index] = _handleTransform.InverseTransformPoint(point);
			}
		}

		return point;
	}
}
