namespace Helper.Patterns.Factory
{
	public class ConcreteCreator<BaseClass, Concrete> : AbstractCreator<BaseClass>  where BaseClass : class
		where Concrete : BaseClass, new() 
	{
		public override BaseClass Create()
		{
			return new Concrete();
		}
	}
}
