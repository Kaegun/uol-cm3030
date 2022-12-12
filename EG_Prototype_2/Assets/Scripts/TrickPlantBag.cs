using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickPlantBag : MonoBehaviour, IInteractable
{
    [SerializeField]
    private TrickPlant _trickPlant;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CanBeInteractedWith()
    {
        return true;
    }

    public void OnPlayerInteract(PlayerController player)
    {
        var trickPlant = Instantiate(_trickPlant, transform.position + new Vector3(1.5f, 0, 1.5f), Quaternion.identity);
        trickPlant.gameObject.transform.localScale = Vector3.one * 0.5f;
    }
}
