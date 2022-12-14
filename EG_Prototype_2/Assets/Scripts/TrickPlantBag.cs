using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickPlantBag : MonoBehaviour, IInteractable
{
    [SerializeField]
    private TrickPlant _trickPlant;

    [SerializeField]
    private Transform _spawnTransform;

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
        var trickPlant = Instantiate(_trickPlant, _spawnTransform.position, Quaternion.identity);
        trickPlant.gameObject.transform.localScale = Vector3.one * 0.5f;
    }
}
