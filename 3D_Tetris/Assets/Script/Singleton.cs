using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Singleton <T> 
{

    public static T Instanse { get; private set; } 

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this as T;
        else
            Destroy(this);
    }

}
