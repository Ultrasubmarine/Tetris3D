using Helper.Patterns.FSM;
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

		Debug.Log("Generate new ONN");
		var element = _generator.GenerationNewElement( _elementManager.transform);
		
		ElementManager.NewElement = element;
		ElementManager.NewElement.MyTransform.parent = _elementManager.transform;
		
		Debug.Log("Generate new element");
//		_OnEnter.Invoke();
		
		_FSM.SetNewState( TetrisState.Drop);
	}

	public override void Exit(TetrisState last) {
	}
	
}
