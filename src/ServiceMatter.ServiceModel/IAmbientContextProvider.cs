namespace ServiceMatter.ServiceModel
{
    public interface IAmbientContextProvider<T> : IAmbientContextProvider where T : class
    {
        new T Create();
    }

    public interface IAmbientContextProvider
    {
        object Create();
    }
}
