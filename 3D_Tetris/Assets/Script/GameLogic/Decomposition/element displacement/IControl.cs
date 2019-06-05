using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlBase<T>: MonoBehaviour {

    public void Action( T parametr) {
        if (CheckOpportunity(parametr)) {
            Logic(parametr);
            Vizual(parametr);
        }

    }

    abstract protected bool CheckOpportunity( T parametr);

    abstract protected void Logic( T parametr);
    abstract protected void Vizual( T parametr);
    
}
