using UnityEngine;
using UnityEngine.Assertions;

public class SpiritBody : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer _meshRenderer;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_meshRenderer, Utility.AssertNotNullMessage(nameof(_meshRenderer)));
	}

	public void SetMaterial(Material material, bool additive = false)
	{
		if (additive)
			_meshRenderer.materials = new Material[] { _meshRenderer.material, material, };
		else
			_meshRenderer.material = material;
	}
}
