using UnityEngine;

public abstract class PickUpBase : MonoBehaviour, IPickUp
{
    [Header("Pickup Transform")]
    [SerializeField]
    protected Vector3 _adjustmentPosition;

    [SerializeField]
    protected Vector3 _adjustmentRotation;

    [SerializeField]
    protected float _adjustmentScaleFactor = 100f;

    [Header("Despawns")]
    [SerializeField]
    protected bool _despawns = false;

    [SerializeField]
    protected float _despawnTimeout = 5.0f;

    [Header("Pickup Animation")]
    [SerializeField]
    protected bool _playAnimation = false;

    public Transform Transform => transform;

    public virtual bool CanBeDropped => _held;

	public virtual bool CanBePickedUp
	{
		get { return !_held && _canBePickedUp; }
		set { _canBePickedUp = value; }
	}

	public virtual bool Despawns
	{
		get { return _despawns; }
		set { _despawns = value; }
	}

	public virtual bool PlayAnimation => _playAnimation;

    protected bool _held = false, _canBePickedUp = true;
    private float _despawnTimer;

    public virtual void OnDrop(bool destroy = false)
    {
        _held = false;
        //	Set parent to null if its being held
        transform.SetParent(null);
        if (destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            //	Reset the despawn timer
            _despawnTimer = 0.0f;
            //	Randomize Y rotation of dropped object - could be parameterized
            transform.SetPositionAndRotation(new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity.RandomizeY());
        }
    }

    public virtual void OnPickUp(Transform pickupTransform)
    {
        _held = true;

        transform.SetParent(pickupTransform, false);

        transform.localPosition = Vector3.zero + _adjustmentPosition;
        transform.localRotation = Quaternion.Euler(_adjustmentRotation.x, _adjustmentRotation.y, _adjustmentRotation.z);
        transform.localScale *= _adjustmentScaleFactor;
    }

    private void Update()
    {
        if (!_held && _despawns)
        {
            _despawnTimer += Time.deltaTime;
            if (_despawnTimer > _despawnTimeout)
            {
                Destroy(gameObject);
            }
        }
    }
}
