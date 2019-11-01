namespace Helper.Patterns.Factory
{
    public abstract class AbstractCreator<T> where T : class
    {
        public abstract T Create();
    }
}