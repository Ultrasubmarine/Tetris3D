using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnitEventStr : UnityEventT<string> { }

[Serializable]
public class UnitEventMove : UnityEventT<move> { }


public class ListStr :  Listener<string> {

    [Header(" переопределенный класс ( здесь)")]
    public UnitEventStr intInvoke;

    [Header(" шаблон с параметрами UnityEvent ")]
    public UnityEvent<string> invokeShapl;
    [Header(" переопределенный класс ( здесь)")]
    public UnitEventMove invokeMove;

    [Serializable]
    public class UnitEventMove : UnityEventT<int> { }

    [Header(" переопределенный класс (  внутри класса здесь)")]
    public UnitEventMove invokeInt;

}
