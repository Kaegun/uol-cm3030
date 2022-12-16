using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeafBlower : MonoBehaviour, IPickUp, IInteractable
{
    [SerializeField]
    private float _cooldownDuration;

    private float _cooldownProgress;

    [SerializeField]
    private float _range;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_cooldownProgress > 0)
        {
            _cooldownProgress -= Time.deltaTime;
        }
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
        return gameObject;
    }

    public bool CanBeInteractedWith()
    {
        return _cooldownProgress <= 0;
    }

    public void OnPlayerInteract(PlayerController player)
    {
        var spirits = Physics.OverlapSphere(transform.position, _range).
            Where(s => (s.GetComponent<Spirit>() != null && s.GetComponent<Spirit>().CanBeRepelled())).
            Select(s => s.GetComponent<Spirit>()).
            ToList();
        foreach (var spirit in spirits)
        {
            spirit.Repel(transform.position);
        }
        _cooldownProgress = _cooldownDuration;
    }


}
