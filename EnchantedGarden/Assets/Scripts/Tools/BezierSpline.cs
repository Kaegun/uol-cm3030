using System;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
	[SerializeField]
	private Vector3[] _points;

	[Header("Colour")]
	[SerializeField]
	private Color _lineColour = Color.gray;

	[SerializeField]
	private Color _curveColour = Color.white;

	[SerializeField]
	private Color _velocityColour = Color.green;

	[SerializeField]
	private Texture2D _texture;

	[SerializeField]
	private float _width = 2f;

	public Vector3[] Points => _points;

	public Color LineColour => _lineColour;

	public Color CurveColour => _curveColour;

	public Color VelocityColour => _velocityColour;

	public Texture2D Texture => _texture;

	public float Width => _width;

	public int CurveCount => (_points.Length - 1) / 3;

	public void Reset()
	{
		_points = new Vector3[] {
					new Vector3(1f, 0f, 0f),
					new Vector3(2f, 0f, 0f),
					new Vector3(3f, 0f, 0f),
					new Vector3(4f, 0f, 0f),
		};
	}

	public void AddCurve()
	{
		var length = _points.Length;
		var point = _points[length - 1];
		Array.Resize(ref _points, length + 3);
		length = _points.Length;
		for (int i = 3; i >= 1; i--)
		{
			point.x += 1.0f;
			_points[length - i] = point;
		}
	}

	public Vector3 GetWorldPoint(float t)
	{
		var i = CalculateCurveOffset(ref t);
		return BezierUtility.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t);
	}

	public Vector3 GetWorldVelocity(float t)
	{
		var i = CalculateCurveOffset(ref t);
		return BezierUtility.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t);
	}

	public Vector3 GetWorldDirection(float t) => GetWorldVelocity(t).normalized;

	public Vector3 GetPoint(float t)
	{
		var i = CalculateCurveOffset(ref t);
		return transform.TransformPoint(BezierUtility.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t));
	}

	public Vector3 GetVelocity(float t)
	{
		var i = CalculateCurveOffset(ref t);
		return transform.TransformPoint(BezierUtility.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) - transform.position;
	}

	public Vector3 GetDirection(float t) => GetVelocity(t).normalized;

	private int CalculateCurveOffset(ref float t)
	{
		int i;
		if (t >= 1f)
		{
			t = 1f;
			i = _points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}

		return i;
	}
}
