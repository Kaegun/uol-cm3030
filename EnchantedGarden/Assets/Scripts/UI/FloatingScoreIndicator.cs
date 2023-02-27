using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloatingScoreIndicator : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;


    public void SetProperties (float score)
    {
        _text.text = $"+ {score}";
    }
}
