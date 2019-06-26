using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlBase<T>: MonoBehaviour {

    public bool Action( T parametr) {
        if (CheckOpportunity(parametr)) {
            Logic(parametr);
            Vizual(parametr);
            return true;
        }
        return false;

    }

    abstract protected bool CheckOpportunity( T parametr);

    abstract protected void Logic( T parametr);
    abstract protected void Vizual( T parametr);
    
}
