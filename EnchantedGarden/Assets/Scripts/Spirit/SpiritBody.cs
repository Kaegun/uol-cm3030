using UnityEngine;
using UnityEngine.Assertions;

public class SpiritBody : MonoBehaviour
{
	private MeshRenderer _meshRenderer;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsTrue(TryGetComponent(out _meshRenderer), Utility.AssertNotNullMessage(nameof(_meshRenderer)));
	}

	public void SetMaterial(Material material, bool additive = false)
	{
		Debug.Log($"SetMaterial: [{_meshRenderer.materials.Length}] | [{material.name}] | [{additive}]");
		if (additive)
			_meshRenderer.materials = new Material[] { _meshRenderer.material, material, };
		else
			_meshRenderer.material = material;

		Debug.Log($"After SetMaterial: [{_meshRenderer.materials.Length}] | [{_meshRenderer.material.name}]");
	}
}
