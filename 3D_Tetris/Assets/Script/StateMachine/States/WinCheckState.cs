using Helper.Patterns.FSM;
using UnityEngine;

public class WinCheckState : AbstractState<TetrisState>
{
    private Score _score;
    private GameCamera _gameCamera;
    
    public WinCheckState()
    {
        _score = RealizationBox.Instance.score;
        _gameCamera = RealizationBox.Instance.gameCamera;
    }

    public override void Enter(TetrisState last)
    {
        base.Enter(last);
        
        if (_score.CheckWin())
            Debug.Log("Win");
        else
        {
            _gameCamera.SetStabilization();
            _FSM.SetNewState(TetrisState.GenerateElement);
        }
    }

    public override void Exit(TetrisState last)
    {
    }
}