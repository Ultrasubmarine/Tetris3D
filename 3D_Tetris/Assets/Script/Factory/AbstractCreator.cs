namespace Helper.Patterns.Factory
{
	public abstract class AbstractCreator <T> where T: class
	{
		abstract public T Create();
	}
}
