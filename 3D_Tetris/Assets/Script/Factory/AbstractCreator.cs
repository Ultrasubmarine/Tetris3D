using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCreator <T> where T: class
{
	abstract public T Create();
}
