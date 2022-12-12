using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickUp
{
    bool CanBePickedUp();
    bool CanBeDropped();
    void OnPickUp();
    void OnDrop();
}
