using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPatch : MonoBehaviour
{
    [SerializeField]
    private bool _containsPlant;

    public bool ContainsPlant()
    {
        return _containsPlant;
    }

    public void PlantPlant()
    {
        _containsPlant = true;
    }

    public void RemovePlant()
    {
        _containsPlant = false;
    }
}
