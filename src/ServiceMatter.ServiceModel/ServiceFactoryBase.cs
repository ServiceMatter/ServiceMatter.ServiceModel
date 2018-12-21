using System.Diagnostics;

namespace ServiceMatter.ServiceModel
{
    public abstract class ServiceFactoryBase<TContext> : IServiceFactory where TContext : class
    {
        private readonly IAmbientContextProvider<TContext> _ambientContextProvider = null;
        private TContext _context = null;

        protected TContext Context => _context ?? (_context = _ambientContextProvider?.Create());

        protected ServiceFactoryBase(TContext context) : this(null,context) 
        {
        }

        protected ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider) :this(contextProvider,null)
        {
        }

        protected ServiceFactoryBase(IAmbientContextProvider<TContext> contextProvider, TContext context)
        {
            Debug.Assert(context == null || contextProvider == null); //Cannot both be provided

            _context = context;
            _ambientContextProvider = contextProvider;
        }



        public abstract IContract Create<IContract>() where IContract : class;
    }
}
