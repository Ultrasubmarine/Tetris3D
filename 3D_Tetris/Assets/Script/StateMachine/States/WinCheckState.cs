using Helper.Patterns.FSM;
using UnityEngine;

public class WinCheckState : AbstractState<TetrisState>
{
    private Score _score;

    public WinCheckState()
    {
        _score = RealizationBox.Instance.score;
    }

    public override void Enter(TetrisState last)
    {
        base.Enter(last);
        
        if (_score.CheckWin())
            Debug.Log("Win");
        else
            _FSM.SetNewState(TetrisState.GenerateElement);
    }

    public override void Exit(TetrisState last)
    {
    }
}