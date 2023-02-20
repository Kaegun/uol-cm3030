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

	private BezierSpline _spline;

	private void Start()
	{
		Assert.IsTrue(TryGetComponent(out _spline), Utility.AssertNotNullMessage(nameof(_spline)));
		Assert.IsTrue(TryGetComponent<MeshFilter>(out var meshFilter), Utility.AssertNotNullMessage(nameof(meshFilter)));
		var mesh = meshFilter.mesh != null ? meshFilter.mesh : new Mesh();
		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var uvs = new List<Vector2>();

		var numQuads = _spline.Points.Length * _quadsPerPoint;

		for (int i = 0; i <= numQuads; i++)
		{
			vertices.AddRange(MakeQuad(i));
			uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), });
			triangles.AddRange(new int[] { i * 4 + 0, i * 4 + 2, i * 4 + 1, i * 4 + 3, i * 4 + 1, i * 4 + 2 });

			DrawLine(i);
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.Optimize();
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;

		Debug.Log($"# UV: {mesh.uv.Length}");
		foreach (var uv in mesh.uv)
		{
			Debug.Log(uv);
		}
	}

	private void Update()
	{
	}

	private Vector3[] MakeQuad(int step)
	{
		var t = step * StepSize;
		var point = _spline.GetWorldPoint(t);
		var nextPoint = _spline.GetWorldPoint(t + StepSize);
		var direction = _spline.GetWorldDirection(t);
		var nextDirection = _spline.GetWorldDirection(t + StepSize);

		return new Vector3[]
				{
					point,
					nextPoint,
					point + (Vector3.Cross(direction, Vector3.up)) * _width,
					nextPoint + (Vector3.Cross(nextDirection, Vector3.up)) * _width,
				};
	}

	private void DrawLine(int i) => Debug.DrawLine(_spline.GetWorldPoint(i * StepSize), _spline.GetWorldPoint(i * StepSize + StepSize), Color.blue);

	private float StepSize => 1f / (_spline.Points.Length * _quadsPerPoint);
}

