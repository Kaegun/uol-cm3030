using System;
using UnityEngine;

//	REF: https://catlikecoding.com/unity/tutorials/curves-and-splines/
public class BezierSpline : MonoBehaviour
{
	public enum ControlPointMode
	{
		Free,
		Aligned,
		Mirrored,
	}

	[SerializeField]
	private Vector3[] _points;

	[SerializeField]
	private ControlPointMode[] _modes;

	[SerializeField]
	private bool _loop;

	[Header("Style")]
	[SerializeField]
	private Color _lineColour = Color.gray;

	[SerializeField]
	private Color[] _pointColours = { Color.white, Color.yellow, Color.cyan };

	[SerializeField]
	private Color _velocityColour = Color.green;

	[SerializeField]
	private Texture2D _texture;

	[SerializeField]
	private float _width = 2f;

	public bool Loop
	{
		get { return _loop; }
		set
		{
			_loop = value;
			if (value)
			{
				_modes[_modes.Length - 1] = _modes[0];
				SetControlPoint(0, _points[0]);
			}
		}
	}

	public Color LineColour => _lineColour;

	public Color[] PointColours => _pointColours;

	public Color VelocityColour => _velocityColour;

	public Texture2D Texture => _texture;

	public float Width => _width;

	public int CurveCount => (_points.Length - 1) / 3;

	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 delta = point - _points[index];
			//	Wrap the points correctly when it's a loop
			if (_loop)
			{
				if (index == 0)
				{
					_points[1] += delta;
					_points[_points.Length - 2] += delta;
					_points[_points.Length - 1] = point;
				}
				else if (index == _points.Length - 1)
				{
					_points[0] = point;
					_points[1] += delta;
					_points[index - 1] += delta;
				}
				else
				{
					_points[index - 1] += delta;
					_points[index + 1] += delta;
				}
			}
			else
			{
				if (index > 0)
					_points[index - 1] += delta;
				if (index + 1 < _points.Length)
					_points[index + 1] += delta;
			}
		}

		_points[index] = point;
		EnforceMode(index);
	}

	public Vector3 GetControlPoint(int index) => _points[index];

	public int ControlPointCount => _points.Length;

	//	The number of modes = the number of curves
	public ControlPointMode GetControlPointMode(int index) => _modes[(index + 1) / 3];

	//	The number of modes = the number of curves
	public void SetControlPointMode(int index, ControlPointMode mode)
	{
		int modeIndex = (index + 1) / 3;
		_modes[modeIndex] = mode;
		//	Set the first and last modes correctly when it's a loop
		if (_loop)
		{
			if (modeIndex == 0)
				_modes[_modes.Length - 1] = mode;
			else if (modeIndex == _modes.Length - 1)
				_modes[0] = mode;
		}
		EnforceMode(index);
	}

	public void Reset()
	{
		_points = new Vector3[] {
					new Vector3(1f, 0f, 0f),
					new Vector3(2f, 0f, 0f),
					new Vector3(3f, 0f, 0f),
					new Vector3(4f, 0f, 0f),
		};

		_modes = new ControlPointMode[] { ControlPointMode.Free, ControlPointMode.Free };
	}

	public void AddCurve()
	{
		var length = _points.Length;
		var point = _points[length - 1];

		//	Resize the spline array
		Array.Resize(ref _points, length + 3);
		length = _points.Length;
		for (int i = 3; i >= 1; i--)
		{
			point.x += 1.0f;
			_points[length - i] = point;
		}

		//	Resize the control point mode array
		Array.Resize(ref _modes, _modes.Length + 1);
		_modes[_modes.Length - 1] = _modes[_modes.Length - 2];
		EnforceMode(_points.Length - 4);

		if (_loop)
		{
			_points[_points.Length - 1] = _points[0];
			_modes[_modes.Length - 1] = _modes[0];
			EnforceMode(0);
		}
	}

	public void ChangeDirection()
	{
		var points = new Vector3[_points.Length];
		for (int i = _points.Length - 1, j = 0; i >= 0; i--, j++)
		{
			points[j] = _points[i];
		}
		_points = points;
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

	private void EnforceMode(int index)
	{
		int modeIndex = (index + 1) / 3;
		ControlPointMode mode = _modes[modeIndex];
		if (mode == ControlPointMode.Free || !_loop && (modeIndex == 0 || modeIndex == _modes.Length - 1))
			return;

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex)
		{
			fixedIndex = middleIndex - 1 < 0 ? _points.Length - 2 : middleIndex - 1;
			enforcedIndex = middleIndex + 1 >= _points.Length ? 1 : middleIndex + 1;
		}
		else
		{
			fixedIndex = middleIndex + 1 >= _points.Length ? 1 : middleIndex + 1;
			enforcedIndex = middleIndex - 1 < 0 ? _points.Length - 2 : middleIndex - 1;
		}

		Vector3 middle = _points[middleIndex];
		Vector3 enforcedTangent = middle - _points[fixedIndex];

		if (mode == ControlPointMode.Aligned)
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _points[enforcedIndex]);

		_points[enforcedIndex] = middle + enforcedTangent;
	}
}
