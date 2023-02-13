using System.Linq;
using UnityEngine;

public class PesticideSpray : PickUpBase, ICombinable
{
    [SerializeField]
    private GameObject _contents;

    [SerializeField]
    private bool _full;

    [SerializeField]
    private float _combinationThreshold = 2f;
    private float _combinationProgress = 0f;

    public bool CanUseSpray => _full;

    public void UseSpray()
    {
        _full = false;
        _contents.SetActive(false);
    }

    public void Combining()
    {
        Debug.Log("Combining!");
        _combinationProgress += Time.deltaTime;
        if (_combinationProgress >= _combinationThreshold)
        {
            OnCombine();
        }
    }

    public void OnCombine()
    {
        _full = true;
        _contents.SetActive(true);
        _combinationProgress = 0f;
    }

    public bool CanBeCombined => _held && !_full;

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
