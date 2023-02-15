using System.Linq;
using UnityEngine;

public class PesticideSpray : PickUpBase, ICombinable, IInteractor
{
    [SerializeField]
    private GameObject _contents;

    [SerializeField]
    private bool _full;

    [SerializeField]
    private float _combinationThreshold = 1f;
    private float _combinationProgress = 0f;

    public bool CanUseSpray => _full;

    public void UseSpray()
    {
        _full = false;
        _contents.SetActive(false);
    }

    public bool Combining()
    {
        _combinationProgress += Time.deltaTime;
        if (_combinationProgress >= _combinationThreshold)
        {
            OnCombine();
            return true;
        }
        return false;
    }

    public void OnCombine()
    {
        _full = true;
        _contents.SetActive(true);
        _combinationProgress = 0f;
    }

    public bool CanBeCombined => _held && !_full;

    public GameObject GameObject => gameObject;

    public bool CanInteractWith(IInteractable interactable)
    {
        switch (interactable)
        {
            case Cauldron _:
                return CanBeCombined;
            case Spirit _:
                return CanUseSpray;
            default:
                return false;
        }
    }

    public void OnInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Spirit _:
                UseSpray();
                break;
            default:
                break;
        }
    }

    public bool DestroyAfterInteract(IInteractable interactable)
    {
        switch (interactable)
        {
            case Spirit _:
                return true;
            default:
                return false;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _full = false;
        _contents.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
