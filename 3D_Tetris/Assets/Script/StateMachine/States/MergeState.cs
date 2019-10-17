using Helper.Patterns.FSM;

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
