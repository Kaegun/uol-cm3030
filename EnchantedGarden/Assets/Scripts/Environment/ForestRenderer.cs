using Boo.Lang;
using System.Linq;
using UnityEngine;

public class ForestRenderer : MonoBehaviour
{
	[SerializeField]
	private GameObject _treePrefab;

	[SerializeField]
	private float _seed = -1.0f;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float _density;

	// Start is called before the first frame update
	private void Start()
	{
		//  Clamp density to 0 - 1
		_density = Mathf.Clamp(_density, 0, 1);
		var mesh = GetComponent<MeshFilter>().sharedMesh;
		Debug.Log($"{name}: {mesh.bounds}");
		var x = mesh.bounds.size.x * transform.localScale.x;
		var z = mesh.bounds.size.z * transform.localScale.z;

		var area = x * z;
		//  Calculate the tree's area
		var treeArea = GetAreaOfObject(_treePrefab);

		Debug.Log($"_density:{_density} | area:{area} | treeArea:{treeArea} | number:{Mathf.CeilToInt(area / treeArea * _density)}");

		int number = Mathf.CeilToInt(area / treeArea * _density),
			i = 0;
		List<Vector3> positions = new List<Vector3>();
		var guard = 0;
		while (i < number)
		{
			var pos = new Vector3(Random.Range(-x / 2, x / 2), 0, Random.Range(-z / 2, z / 2));
			if (positions.Where(p => (pos - p).sqrMagnitude / 4 < treeArea / 4).Count() == 0 && guard++ < 5)
			{
				Debug.Log(pos);
				Instantiate(_treePrefab, transform.position + pos, Quaternion.identity);
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
		Debug.Log($"{meshFilters.Length}");

		foreach (var mf in meshFilters)
		{
			x = Mathf.Max(x, mf.sharedMesh.bounds.size.x);
			z = Mathf.Max(z, mf.sharedMesh.bounds.size.z);
		}

		Debug.Log($"x: {x} | z:{z}");

		return x * z;
	}
}
