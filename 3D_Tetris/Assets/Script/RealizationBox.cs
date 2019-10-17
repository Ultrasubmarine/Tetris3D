using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealizationBox : Singleton<RealizationBox>
{

	[SerializeField] PlaneMatrix _Matrix;
	[SerializeField] Generator _Ganerator;
	[SerializeField] ElementManager _ElementManager;
	[SerializeField] private TetrisFSM _FSM;
	public PlaneMatrix Matrix() { return _Matrix; }
	public Generator ElementGenerator() { return _Ganerator; }

	public ElementManager ElementManager() { return _ElementManager; }
	public TetrisFSM FSM => _FSM;
}
