using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Herb : MonoBehaviour, IPickUp
{
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
        var cauldron = Physics.OverlapSphere(transform.position, 2.0f).
            Where(c => c.GetComponent<Cauldron>() != null).
            Select(c => c.GetComponent<Cauldron>()).
            ToList();
        if (cauldron.Count > 0)
        {
            cauldron[0].AddHerb();
            Destroy(gameObject);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public void OnPickUp()
    {

    }

    public GameObject PickUpObject()
    {
        return gameObject;
    }
}
