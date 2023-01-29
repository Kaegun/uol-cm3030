using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpStore : MonoBehaviour, IPickUp
{
    [SerializeField]
    private GameObject _pickUp;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanBeDropped()
    {
        return true;
    }

    public bool CanBePickedUp()
    {
        return true;
    }

    public void OnDrop()
    {
        
    }

    public void OnPickUp()
    {
        
    }

    public GameObject PickUpObject()
    {
        return Instantiate(_pickUp, transform.position, Quaternion.identity);
    }
}
