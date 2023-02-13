using UnityEngine;
using UnityEngine.Assertions;

/*
 * TODO: Spawn timers?
 * Limit spawn count
 */

public abstract class PickUpSpawnerBase : MonoBehaviour
{
    [Header("Pickup Prefab")]
    [SerializeField]
    private PickUpBase _spawnedPrefab;

    public IPickUp SpawnPickUp()
    {
        var pickUp = Instantiate(_spawnedPrefab);
        Assert.IsNotNull(pickUp);
        return pickUp;
    }
}
