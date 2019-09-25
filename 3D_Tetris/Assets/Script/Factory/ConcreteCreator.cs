using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteCreator<BaseClass, Concrete> : AbstractCreator<BaseClass>  where BaseClass : class
																				where Concrete : BaseClass, new() 
{
	public override BaseClass Create()
	{
		return new Concrete();
	}
}
