using System.Xml.Serialization;
using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class AllElementsDropState : AbstractState<TetrisState>
{
    private ElementManager _elementManager;
    private ElementCleaner _elementCleaner;

    public AllElementsDropState()
    {
        _elementManager = RealizationBox.Instance.elementManager;
        _elementCleaner = RealizationBox.Instance.elementCleaner;
    }

    public override void Enter(TetrisState last)
    {
        _elementCleaner.ClearElementsFromDeletedBlocks();
        _elementCleaner.CutElement();
        _elementManager.StartDropAllElements();
    }

    public override void Exit(TetrisState last)
    {
    }
}