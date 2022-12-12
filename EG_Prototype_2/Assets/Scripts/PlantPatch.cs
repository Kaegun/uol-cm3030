using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPatch : MonoBehaviour
{
    [SerializeField]
    private bool _containsPlant;

    [SerializeField]
    private MeshRenderer _mesh;

    [SerializeField]
    private Material _dirtMaterial;
    [SerializeField]
    private Material _compostMaterial;

    private bool _containsCompost = false;

    public bool ContainsPlant()
    {
        return _containsPlant;
    }

    public void AddPlant()
    {
        _containsPlant = true;
    }

    public void RemovePlant()
    {
        _containsPlant = false;
    }

    public bool ContainsCompost()
    {
        return _containsCompost;
    }

    public void AddCompost()
    {
        _containsCompost = true;
        _mesh.material = _compostMaterial;
    }

    public void RemoveCompost()
    {
        _containsCompost = false;
        _mesh.material = _dirtMaterial;
    }
}
