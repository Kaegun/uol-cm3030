﻿using UnityEngine;
using UnityEngine.Assertions;

public abstract class PickUpBase : MonoBehaviour, IPickUp
{
	[Header("Pickup Transform")]
	[SerializeField]
	protected Vector3 _adjustmentPosition;

	[SerializeField]
	protected Vector3 _adjustmentRotation;

	[SerializeField]
	protected float _adjustmentScaleFactor = 100f;

	[Header("Pickup Indicator")]
	[SerializeField]
	protected Vector3 _indicatorAdjustmentPosition;

	[SerializeField]
	protected Sprite _carryIcon;

	[SerializeField]
	protected Color _carryIconBaseColor = Color.white;

	[SerializeField]
	protected Color _carryIconCombineColor = Color.green;

	[Header("Despawns")]
	[SerializeField]
	protected bool _despawns = false;

	[SerializeField]
	protected float _despawnTimeout = 5.0f;

	[Header("Pickup Animation")]
	[SerializeField]
	protected bool _playAnimation = false;

	public virtual Transform Transform => transform;

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

	private bool _despawned = false;
	public virtual bool Despawned => _despawned;

	public virtual bool PlayAnimation => _playAnimation;

	public virtual Vector3 IndicatorPostion => transform.position + _indicatorAdjustmentPosition;

	public virtual Sprite CarryIcon => _carryIcon;

	public virtual Color CarryIconBaseColor => _carryIconBaseColor;

	public virtual Color CarryIconCombineColor => _carryIconCombineColor;

	protected bool _held = false, _canBePickedUp = true;
	private float _despawnTimer;
	private const float _playerModelScaleFix = 100.0f;

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

			//	There's a problem on the player model that scales the items down by 100
			transform.localScale /= _adjustmentScaleFactor;
		}
	}

	public virtual void OnPickUp(Transform pickupTransform)
	{
		_held = true;
		//	Reset the despawn timer
		_despawnTimer = 0.0f;

		transform.SetParent(pickupTransform, false);

		transform.localPosition = Vector3.zero + _adjustmentPosition;
		transform.localRotation = Quaternion.Euler(_adjustmentRotation.x, _adjustmentRotation.y, _adjustmentRotation.z);
		//	There's a problem on the player model that scales the items down by 100
		transform.localScale *= _adjustmentScaleFactor * _playerModelScaleFix;
	}

	protected virtual void Start()
	{
		Assert.IsNotNull(_carryIcon, Utility.AssertNotNullMessage(nameof(_carryIcon)));
	}

	protected virtual void Update()
	{
		if (!_held && _despawns)
		{
			_despawnTimer += Time.deltaTime;
			if (_despawnTimer > _despawnTimeout)
			{
				_canBePickedUp = false;
				_despawned = true;
				Destroy(gameObject);
			}
		}
	}
}
