using System;
using System.Collections;
using System.Collections.Generic;
using Script.GameLogic.TetrisElement;
using Script.ObjectEngine;
using UnityEngine;

public class RealizationBox : Singleton<RealizationBox>
{

	[SerializeField] PlaneMatrix _Matrix;
	[SerializeField] Generator _Ganerator;
	[SerializeField] ElementManager _ElementManager;
	[SerializeField] private TetrisFSM _FSM;
	[SerializeField] private Score _Score;
	[SerializeField] private InfluenceManager _InfluenceManager;
	[SerializeField] private ElementCleaner _elementCleaner;
	public PlaneMatrix Matrix() { return _Matrix; }
	public Generator ElementGenerator() { return _Ganerator; }

	public ElementManager ElementManager() { return _ElementManager; }
	public TetrisFSM FSM => _FSM;
	public Score Score => _Score;


	public InfluenceManager InfluenceManager => _InfluenceManager;
	public ElementCleaner ElementCleaner => _elementCleaner;

	private void Start()
	{
		ElementData.Loader = () =>
		{
			return _Ganerator.GenerationNewElement(_ElementManager.transform);
		};
	}
}
