using System.Collections;
using System.Collections.Generic;
using Script.GameLogic.TetrisElement;
using UnityEngine;
using UnityEngine.Serialization;

public class BBC : Singleton<BBC>
{
    [SerializeField] private PlaneMatrix _Matrix;
    [FormerlySerializedAs("_ElementManager")] [SerializeField] private ElementDropper elementDropper;
    [SerializeField] private Generator _Generator;

    private PlaneMatrix Matrix()
    {
        return _Matrix;
    }

    private ElementDropper ElementManager()
    {
        return elementDropper;
    }

    private Generator Generator()
    {
        return _Generator;
    }
}