namespace PracticalModules.Patterns.Singleton
{
    public class SingletonClass<TInstance> where TInstance : class, new()
    {
        public static TInstance Instance { get; } = new TInstance();
    }
}