﻿using System;
using UnityEngine;

//[Obsolete("Don't think we need this anymore")]
//public class CompostBin : MonoBehaviour, IPickUp
//{
//	[SerializeField]
//	private Compost _compost;

//	[SerializeField]
//	private float _cooldownDuration;
//	private float _cooldownProgress;

//	[SerializeField]
//	private MeshRenderer _compostMesh;

//	[SerializeField]
//	private Transform _spawnTransform;

//	//	TODO: Should this be accessed from outside?
//	public bool OnCooldown => _cooldownProgress < _cooldownDuration;

//	public bool CanBePickedUp => !OnCooldown;

//	public bool CanBeDropped => true;

//	public bool PlayAnimation => false;

//	public GameObject PickUpObject()
//	{
//		_cooldownProgress = 0;
//		return Instantiate(_compost, _spawnTransform.position, Quaternion.identity).gameObject;
//	}

//	public void OnPickUp(Transform _) { }

//	public void OnDrop(bool despawn = false) { }

//	//	Start is called before the first frame update
//	private void Start()
//	{
//		_cooldownProgress = _cooldownDuration;
//	}

//	//	Update is called once per frame
//	private void Update()
//	{
//		if (OnCooldown)
//		{
//			_cooldownProgress += Time.deltaTime;
//			if (_cooldownProgress > _cooldownDuration)
//			{
//				_cooldownProgress = _cooldownDuration;
//			}
//		}

//		//	Alter opacity of compost in compost bin to signify cooldown
//		var color = _compostMesh.material.color;
//		//	TODO: Can lerp this color for a smoother transition?
//		color.a = OnCooldown ? 0 : 1;
//		_compostMesh.material.color = color;
//	}
//}
