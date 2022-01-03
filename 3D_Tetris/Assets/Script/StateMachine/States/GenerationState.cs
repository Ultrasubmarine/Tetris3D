using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class GenerationState : AbstractState<TetrisState>
{
    private ElementDropper _elementDropper;

    public GenerationState()
    {
        _elementDropper = RealizationBox.Instance.elementDropper;
    }

    public override void Enter(TetrisState last)
    {
        ElementData.LoadNewElement();
        ElementData.newElement.myTransform.parent = _elementDropper.transform;

        _FSM.SetNewState(TetrisState.Drop);
    }

    public override void Exit(TetrisState last)
    {
    }
}