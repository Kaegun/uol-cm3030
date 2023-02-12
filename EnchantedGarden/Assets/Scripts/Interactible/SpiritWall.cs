﻿using UnityEngine;

public class SpiritWall : MonoBehaviour, IPossessable
{
    [SerializeField]
    private bool _isPossessed;

    [SerializeField]
    private GameObject _wallObj;

    public bool CanBePossessed => !_isPossessed;

    // Possession threshold not used so possession complete instantly
    // Change later?
    public bool PossessionCompleted => true;

    public Transform Transform => transform;

    public GameObject GameObject => gameObject;

    public void OnDispossess()
    {
        _isPossessed = false;
        // Destroy grown wall
        _wallObj.SetActive(false);
        // Score points
    }

    public void OnPossessionStarted(Spirit possessor)
    {
        _isPossessed = true;

    }

    public void WhileCompletingPossession(Spirit possessor)
    {

    }

    public void OnPossessionCompleted(Spirit possessor)
    {
        // Grow wall
        _wallObj.SetActive(true);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _wallObj.SetActive(false);
    }
}