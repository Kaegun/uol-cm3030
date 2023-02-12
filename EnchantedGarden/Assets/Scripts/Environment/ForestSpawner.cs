using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		var mesh = GetComponent<MeshFilter>().sharedMesh;
		var x = mesh.bounds.size.x * transform.localScale.x;
		var z = mesh.bounds.size.z * transform.localScale.z;

		var area = x * z;
		//  Calculate the average tree area
		float treeArea = 0.0f;
		foreach (var t in _treePrefabs)
		{
			treeArea += GetAreaOfObject(t);
		}
		treeArea /= (_treePrefabs.Length ^ 2);

		int number = Mathf.CeilToInt(area / (treeArea / 4) * _density),
			i = 0;
		List<Vector3> positions = new List<Vector3>();
		var guard = 0;
		while (i < number)
		{
			//	Select tree
			var tree = _treePrefabs[Random.Range(0, _treePrefabs.Length - 1)];
			var pos = new Vector3(Random.Range(-x / 2, x / 2), 0, Random.Range(-z / 2, z / 2));
			if (positions.Where(p => (pos - p).sqrMagnitude / 4 < treeArea / 4).Count() == 0 || guard++ > 5)
			{
				var newTree = Instantiate(tree, transform.position + transform.rotation * pos, Quaternion.identity);
				newTree.transform.SetParent(transform);
				positions.Add(pos);
				i++;
				guard = 0;
			}
		}
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
}
