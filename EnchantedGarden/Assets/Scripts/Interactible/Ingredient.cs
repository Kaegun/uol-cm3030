using UnityEngine;
using UnityEngine.Assertions;

//	Logic in base
public class Ingredient : PickUpBase, IInteractor
{
    [SerializeField]
    private GameObject[] _ingredientPrefabs;

    public GameObject GameObject => gameObject;

    public bool CanInteractWith(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                return true;
            default:
                return false;
        }
    }

    public bool DestroyAfterInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                return true;
            default:
                return false;
        }
    }

    public void OnInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(_ingredientPrefabs);
        Assert.IsTrue(_ingredientPrefabs.Length > 0);

        Debug.Log("Where are the ingredients?");
        Instantiate(_ingredientPrefabs[Random.Range(0, _ingredientPrefabs.Length - 1)], transform);
    }
}
