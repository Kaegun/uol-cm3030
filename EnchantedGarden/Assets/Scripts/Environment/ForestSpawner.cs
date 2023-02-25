using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class ForestSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _treePrefabs;

	[SerializeField]
	private int _seed = -1;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float _density;

	// Start is called before the first frame update
	private void Start()
	{
		if (_seed > 0)
			Random.InitState(_seed);

		//  Clamp density to 0 - 1
		_density = Mathf.Clamp(_density, 0, 1);
		if (TryGetComponent<MeshFilter>(out var meshFilter))
		{
			var mesh = meshFilter.sharedMesh;

			//	Calculate average radius of the tree prefabs
			float radius = 0.0f;
			for (int i = 0; i < _treePrefabs.Length; i++)
			{
				//	Approximate the radius using the square of the area
				radius += Mathf.Sqrt(GetAreaOfObject(_treePrefabs[i]));
			}
			var avgRadius = radius / _treePrefabs.Length;

			//	Track spawned trees to avoid overlaps
			List<Vector3> positions = new List<Vector3>();

			//	Need to split the mesh into quads
			// # Quads - Account for fewer tris to make full quads
			var numQuads = mesh.triangles.Length - mesh.triangles.Length % 4;
			var totalTrees = 0;
			for (int i = 0; i < numQuads; i += 4)
			{
				totalTrees += FillQuad(GetQuad(i, mesh), avgRadius, positions);
			}
			Debug.Log($"Spawned: {totalTrees} trees!");
		}
		else
		{
			Assert.IsNull(meshFilter, Utility.AssertNotNullMessage(nameof(meshFilter)));
		}

		//	Re-randomize the seed after spawning the trees
		if (_seed > 0)
			Random.InitState(System.Environment.TickCount);
	}

	// Update is called once per frame
	private void Update() { }

	private float GetAreaOfObject(GameObject treePrefab)
	{
		float x = 0f,
			 z = 0f;

		//	Calculate the area of the object
		var meshFilters = treePrefab.GetComponentsInChildren<MeshFilter>();

		foreach (var mf in meshFilters)
		{
			x = Mathf.Max(x, mf.sharedMesh.bounds.size.x * treePrefab.transform.localScale.x);
			z = Mathf.Max(z, mf.sharedMesh.bounds.size.z * treePrefab.transform.localScale.z);
		}

		return x * z;
	}

	private int FillQuad(Vector4 quad, float avgRadius, List<Vector3> positions)
	{
		var area = quad.z * quad.w;

		int number = Mathf.CeilToInt(area / avgRadius * _density),
				i = 0, guard = 0, spawns = 0;

		GameObject tree = null;
		float radius = 0.0f;
		while (i < number)
		{
			//	Select tree
			if (tree == null)
			{
				tree = _treePrefabs[Random.Range(0, _treePrefabs.Length)];
				radius = Mathf.Sqrt(GetAreaOfObject(tree)) / 2;
			}
			var pos = new Vector3(quad.x - quad.z / 2 + Random.Range(-quad.z / 2, quad.z / 2), 0, quad.y - quad.w / 2 + Random.Range(-quad.w / 2, quad.w / 2));
			if (guard++ < 5)
			{
				if (positions.All(p => (pos - p).magnitude > radius))
				{
					var newTree = Instantiate(tree, transform.position + transform.rotation * pos, Quaternion.identity);
					newTree.transform.SetParent(transform);
					positions.Add(pos);
					i++;
					guard = 0;
					spawns++;
					tree = null;
				}
			}
			else
			{
				guard = 0;
				//	Skip it if we can't find a place to plant it.
				i++;
			}
		}

		return spawns;
	}

	private Vector4 GetQuad(int startIdx, Mesh mesh)
	{
		float minX = float.MaxValue, minZ = float.MaxValue,
			maxX = float.MinValue, maxZ = float.MinValue;
		for (int i = startIdx; i < startIdx + 4; i++)
		{
			minX = Mathf.Min(mesh.vertices[mesh.triangles[i]].x * transform.localScale.x, minX);
			minZ = Mathf.Min(mesh.vertices[mesh.triangles[i]].z * transform.localScale.z, minZ);

			maxX = Mathf.Max(mesh.vertices[mesh.triangles[i]].x * transform.localScale.x, maxX);
			maxZ = Mathf.Max(mesh.vertices[mesh.triangles[i]].z * transform.localScale.z, maxZ);
		}

		return new Vector4(maxX, maxZ, maxX - minX, maxZ - minZ);
	}
}
