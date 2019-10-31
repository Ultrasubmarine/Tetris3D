using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class GenerationState : AbstractState<TetrisState> 
{
	Generator _generator;
	ElementManager _elementManager;

	public GenerationState() {
		_generator = RealizationBox.Instance.ElementGenerator();
		_elementManager = RealizationBox.Instance.ElementManager();
	}

	public override void Enter(TetrisState last) {

		ElementData.LoadNewElement();
		ElementData.NewElement.MyTransform.parent = _elementManager.transform;

		_FSM.SetNewState( TetrisState.Drop);
	}

	public override void Exit(TetrisState last) {
	}
	
}
