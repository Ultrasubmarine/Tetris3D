using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;

public class MergeState : AbstractState<TetrisState>
{
    ElementManager _elementManager;
    
    public MergeState()
    {
        _elementManager = RealizationBox.Instance.ElementManager();        
    }
    public override void Enter(TetrisState last)
    {
        _elementManager.MergeNewElement();
        _FSM.SetNewState(TetrisState.Collection);
    }

    public override void Exit(TetrisState last)
    {
    }
}
