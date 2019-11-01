using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlBase<T> : MonoBehaviour
{
    public bool Action(T parametr)
    {
        if (CheckOpportunity(parametr))
        {
            Logic(parametr);
            Vizual(parametr);
            return true;
        }

        return false;
    }

    protected abstract bool CheckOpportunity(T parametr);

    protected abstract void Logic(T parametr);
    protected abstract void Vizual(T parametr);
}