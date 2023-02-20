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

	public void SetMaterial(Material material)
	{
		_meshRenderer.material = material;
	}
}
