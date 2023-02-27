using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BezierSpline))]
[RequireComponent(typeof(MeshFilter))]
public class SplineMeshGenerator : MonoBehaviour
{
	[SerializeField]
	private int _quadsPerPoint = 10;

	[SerializeField]
	private float _width = 3.0f;

	[SerializeField]
	private float _widthNoiseFactor = 0.5f;

	private BezierSpline _spline;
	private float _adjustedWidth;

	private void Start()
	{
		Assert.IsTrue(TryGetComponent(out _spline), Utility.AssertNotNullMessage(nameof(_spline)));
		if (TryGetComponent<MeshFilter>(out var meshFilter))
		{
			var mesh = meshFilter.mesh != null ? meshFilter.mesh : new Mesh();
			var vertices = new List<Vector3>();
			var triangles = new List<int>();
			var uvs = new List<Vector2>();

			var numQuads = _spline.ControlPointCount * _quadsPerPoint;
			_adjustedWidth = _width * (1f + Random.Range(-_widthNoiseFactor / 2, _widthNoiseFactor / 2));

			for (int i = 0; i <= numQuads; i++)
			{
				vertices.AddRange(MakeQuad(i));
				if (i % 2 == 0)
					uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), });
				else
					uvs.AddRange(new Vector2[] { new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), });
				triangles.AddRange(new int[] { i * 4 + 0, i * 4 + 2, i * 4 + 1, i * 4 + 3, i * 4 + 1, i * 4 + 2 });
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();

			mesh.Optimize();
			mesh.RecalculateNormals();
			meshFilter.mesh = mesh;
		}
		else
		{
			Assert.IsNull(meshFilter, Utility.AssertNotNullMessage(nameof(meshFilter)));
		}
	}

	private Vector3[] MakeQuad(int step)
	{
		var t = step * StepSize;
		var point = _spline.GetWorldPoint(t);
		var nextPoint = _spline.GetWorldPoint(t + StepSize);
		var direction = _spline.GetWorldDirection(t);
		var nextDirection = _spline.GetWorldDirection(t + StepSize);

		var edge = _width * (1f + Random.Range(0, _widthNoiseFactor));
		var ret = new Vector3[]
					{
							point,
							nextPoint,
							point + (Vector3.Cross(direction, Vector3.up)) * _adjustedWidth,
							nextPoint + (Vector3.Cross(nextDirection, Vector3.up)) * edge,
					};
		_adjustedWidth = edge;
		return ret;
	}

	private float StepSize => 1f / (_spline.ControlPointCount * _quadsPerPoint);
}

