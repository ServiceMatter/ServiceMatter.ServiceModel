using System.Diagnostics;
using ServiceMatter.ServiceModel.Configuration;
using ServiceMatter.ServiceModel.Proxy;

namespace ServiceMatter.ServiceModel
{
    public abstract class ServiceFactoryBase<TContext> : IServiceFactory where TContext : class
    {
        private readonly IAmbientContextProvider<TContext> _ambientContextProvider = null;
        private TContext _context = null;

        protected readonly ProxyFactory<TContext> ProxyFactory;

        protected TContext Context => _context ?? (_context = _ambientContextProvider?.Create());

        protected ServiceFactoryBase(TContext context) : this(null, context, ServiceModelConfiguration<TContext>.ProxyConfiguration)
        { }

        protected ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider) : this(contextProvider, null, ServiceModelConfiguration<TContext>.ProxyConfiguration)
        { }

        protected ServiceFactoryBase(TContext context, ProxyFactoryConfiguration<TContext> proxyFactoryConfiguration) : this(null, context, proxyFactoryConfiguration)
        { }

        protected ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider, ProxyFactoryConfiguration<TContext> proxyFactoryConfiguration) : this(contextProvider, null, proxyFactoryConfiguration)
        { }


        protected ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider, TContext context, ProxyFactoryConfiguration<TContext> proxyFactoryConfiguration)
            : this(contextProvider, context, new ProxyFactory<TContext>(proxyFactoryConfiguration))
        { }

        private ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider, TContext context, ProxyFactory<TContext> proxyFactory)
        {
            Debug.Assert(context != null ^ contextProvider != null,$"Eigther a {nameof(context)} or a {nameof(contextProvider)} should be provided. Not both. Nor should both be null.");

            _context = context;
            _ambientContextProvider = contextProvider;
            ProxyFactory = proxyFactory;
        }

        public abstract IContract Create<IContract>() where IContract : class;
    }
}
