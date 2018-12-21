namespace ServiceMatter.ServiceModel
{
    public interface IServiceFactory
    {
        T Create<T>() where T : class;
    }
}
