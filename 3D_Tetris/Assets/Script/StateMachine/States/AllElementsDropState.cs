using Helper.Patterns.FSM;
using UnityEngine;

public class AllElementsDropState : AbstractState<TetrisState>
{
    private ElementManager _elementManager;

    public AllElementsDropState()
    {
        _elementManager = RealizationBox.Instance.ElementManager();
    }
    
    public override void Enter(TetrisState last)
    {
        _elementManager.ClearElementsAfterDeletedBlocks();
        _elementManager.CutElement();
        _elementManager.StartDropAllElements();
    }

    public override void Exit(TetrisState last)
    {
    }
}
