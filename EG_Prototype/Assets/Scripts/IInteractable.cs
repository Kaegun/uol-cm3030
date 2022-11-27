using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable();
    void OnPlayerInteract(PlayerController player);
}
