using UnityEngine;

public class PickUpIndicator : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _icon;

	private Camera _camera;

	public void SetIcon(Sprite icon)
	{
		_icon.sprite = icon;
	}

	public void SetIconColor(Color color)
	{
		_icon.color = color;
	}

	public bool Active => gameObject.activeSelf;

	public void SetActive(bool active) => gameObject.SetActive(active);

	// Start is called before the first frame update
	private void Start()
	{
		//_spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
		//Assert.IsNotNull(_spriteRenderer, Utility.AssertNotNullMessage(nameof(_spriteRenderer)));

		_camera = Camera.main;
	}

	// Update is called once per frame
	private void Update()
	{
		transform.LookAt(_camera.transform.position.ZeroY());
	}
}
