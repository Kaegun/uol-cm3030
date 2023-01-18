﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [SerializeField]
    private GameObject _fireParticles;

    [SerializeField]
    private float _maxFuel = 10f;
    private float _currentFuel;

    [SerializeField]
    private int _maxUses = 5;
    private int _currentUses;

    [SerializeField]
    private float _combineDuration;
    private float _combineProgress;

    // Start is called before the first frame update
    void Start()
    {
        _currentFuel = _maxFuel;
        _currentUses = _maxUses;
    }

    // Update is called once per frame
    void Update()
    {
        _currentFuel -= Time.deltaTime;
        if (_currentFuel < 0 && _fireParticles.activeSelf)
        {
            _fireParticles.SetActive(false);
        }

        var combinables = Physics.OverlapSphere(transform.position, 2f).
            Where(c => c.GetComponent<ICombinable>() != null && c.GetComponent<ICombinable>().CanBeCombined()).
            Select(c => c.GetComponent<ICombinable>()).
            ToList();       

        if (combinables.Count > 0 && CanUseCauldron())
        {
            _combineProgress += Time.deltaTime;
            if (_combineProgress >= _combineDuration)
            {
                UsePotion();
                combinables[0].OnCombine();
                _combineProgress = 0;
            }
        }
        else
        {
            _combineProgress = 0;
        }
    }

    public void AddLog()
    {
        _currentFuel = _maxFuel;
        if (!_fireParticles.activeSelf)
        {
            _fireParticles.SetActive(true);
        }
    }

    public void AddHerb()
    {
        _currentUses = _maxUses;
    }

    public bool CanUseCauldron()
    {
        return _currentUses > 0 && _currentFuel > 0;
    }

    public void UsePotion()
    {
        _currentUses -= 1;
    }
}
