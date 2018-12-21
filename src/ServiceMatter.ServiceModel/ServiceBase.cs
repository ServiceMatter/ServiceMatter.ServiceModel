namespace ServiceMatter.ServiceModel
{
    public abstract class ServiceBase<TContext> where TContext : class
    {
        protected ServiceBase(TContext context, IServiceFactory factory)
        {
            Context = context;
            ServiceFactory = factory;
        }

        protected TContext Context { get; }

        protected IServiceFactory ServiceFactory { get; }
    }
}
