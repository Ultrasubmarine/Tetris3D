using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBC : Singleton<BBC>
{
	[SerializeField] private PlaneMatrix _Matrix;
	[SerializeField] private ElementManager _ElementManager;
	[SerializeField] private Generator _Generator;
	
	private PlaneMatrix Matrix() { return _Matrix;}
	private ElementManager ElementManager () { return _ElementManager; }
	private Generator Generator () { return _Generator; }
	
	
}
