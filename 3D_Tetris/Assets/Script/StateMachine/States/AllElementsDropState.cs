using System.Xml.Serialization;
using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class AllElementsDropState : AbstractState<TetrisState>
{
    private ElementDropper _elementDropper;
    private ElementCleaner _elementCleaner;

    public AllElementsDropState()
    {
        _elementDropper = RealizationBox.Instance.elementDropper;
        _elementCleaner = RealizationBox.Instance.elementCleaner;
    }

    public override void Enter(TetrisState last)
    {
        _elementCleaner.ClearElementsFromDeletedBlocks();
        _elementCleaner.CutElement();
        _elementDropper.StartDropAllElements();
    }

    public override void Exit(TetrisState last)
    {
    }
}