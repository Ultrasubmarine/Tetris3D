using System.Collections;
using System.Collections.Generic;
using Helper.Patterns.FSM;
using UnityEngine;

public class WaitState : AbstractState<TetrisState>
{
    
    public override void Enter(TetrisState last)
    {
        // input On
    }

    public override void Exit(TetrisState last)
    {
       // input off
    }
}
