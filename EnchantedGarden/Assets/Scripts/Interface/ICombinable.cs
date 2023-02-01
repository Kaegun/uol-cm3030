using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombinable
{
    void OnCombine();
    bool CanBeCombined();
}
