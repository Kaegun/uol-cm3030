using System.Collections;
using System.Collections.Generic;
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
