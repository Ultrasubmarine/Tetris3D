using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class GenerationState : AbstractState<TetrisState>
{
    private Generator _generator;
    private ElementManager _elementManager;

    public GenerationState()
    {
        _generator = RealizationBox.Instance.generator;
        _elementManager = RealizationBox.Instance.elementManager;
    }

    public override void Enter(TetrisState last)
    {
        ElementData.LoadNewElement();
        ElementData.NewElement.MyTransform.parent = _elementManager.transform;

        _FSM.SetNewState(TetrisState.Drop);
    }

    public override void Exit(TetrisState last)
    {
    }
}