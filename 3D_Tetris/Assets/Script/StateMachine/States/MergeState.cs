using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;

public class MergeState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    
    public MergeState()
    {
        _matrix = RealizationBox.Instance.matrix;
    }

    public override void Enter(TetrisState last)
    {
        _matrix.BindToMatrix(ElementData.NewElement);
        ElementData.MergeNewElement();
        
        base.Enter(last);
        _FSM.SetNewState(TetrisState.Collection);
    }

    public override void Exit(TetrisState last)
    {
    }
}