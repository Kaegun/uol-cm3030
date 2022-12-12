using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool CanBeInteractedWith();
    void OnPlayerInteract(PlayerController player);
}
