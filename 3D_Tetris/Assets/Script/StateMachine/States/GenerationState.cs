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
        base.Enter(last);
        ElementData.Instance.LoadNewElement();
        if(_elementDropper.transform == null)
        Debug.Log("_elementDropper.transform == null");
        if(ElementData.Instance.newElement.myTransform == null)
            Debug.Log("ElementData.newElement.myTransform  == null");
        ElementData.Instance.newElement.myTransform.parent = _elementDropper.transform;
        
        RealizationBox.Instance.joystick.gameObject.SetActive(true);
        _FSM.SetNewState(TetrisState.Drop);
    }

    public override void Exit(TetrisState last)
    {
    }
}