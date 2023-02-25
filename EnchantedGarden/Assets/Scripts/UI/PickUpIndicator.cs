using UnityEngine;
using UnityEngine.Assertions;

public class PickUpIndicator : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _icon;

	[SerializeField]
	private SpriteRenderer _secondaryIcon;

	private Camera _camera;

	public void SetIcon(Sprite icon)
	{
		_icon.sprite = icon;
	}

	public void SetSecondaryIcon(Sprite icon)
	{
		_secondaryIcon.sprite = icon;
	}

	public void SetIconColor(Color color)
	{
		_icon.color = color;
	}

	public void SetSecondaryIconColor(Color color)
	{
		_secondaryIcon.color = color;
	}

	public bool Active => gameObject.activeSelf;

	public void SetActive(bool active) => gameObject.SetActive(active);

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_icon, Utility.AssertNotNullMessage(nameof(_icon)));
		Assert.IsNotNull(_secondaryIcon, Utility.AssertNotNullMessage(nameof(_secondaryIcon)));

		_camera = Camera.main;
	}

	// Update is called once per frame
	private void Update()
	{
		transform.rotation = Quaternion.Euler(-_camera.transform.rotation.eulerAngles);
	}
}
