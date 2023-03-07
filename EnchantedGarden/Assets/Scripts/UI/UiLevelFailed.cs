using TMPro;
using UnityEngine;

public class UiLevelFailed : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _notEnoughPointsText;

    // Start is called before the first frame update
    private void Start()
    {
        _notEnoughPointsText.gameObject.SetActive(GameManager.Instance.ActiveLevel.CurrentNumberOfPlants > 0);
    }       
}
