using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var spirit = other.gameObject.GetComponent<Spirit>();
        if (spirit != null && spirit.PossessingPlant())
        {
            spirit.StealPossessedPlant();
        }
    }
}
