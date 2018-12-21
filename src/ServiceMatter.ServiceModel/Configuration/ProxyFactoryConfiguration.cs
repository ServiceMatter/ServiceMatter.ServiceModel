using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServiceMatter.ServiceModel.Configuration
{
    public class ProxyFactoryConfiguration<TAmbientContext>
        where TAmbientContext : class
    {
        private IDictionary<Type, object> _proxyConstructors = new Dictionary<Type, object>(100);
        

        public ProxyContractBehavior<T, TAmbientContext> For<T>()
            where T : class
        {
            var type = typeof(T);
            if (!_proxyConstructors.TryGetValue(typeof(T), out var contractConfig))
            {
                Debug.Assert(type.IsInterface);

                contractConfig = new ProxyContractBehavior<T, TAmbientContext>(this);
                _proxyConstructors[type] = contractConfig;
            }

            return (ProxyContractBehavior<T, TAmbientContext>)contractConfig;
        }


        internal T CreateProxy<T>(T service, TAmbientContext context)
            where T : class
        {
            var type = typeof(T);
            if (_proxyConstructors.TryGetValue(type, out var contractConfig))
            {
                var bhvr = contractConfig as ProxyContractBehavior<T, TAmbientContext>;

                return bhvr.Create(service,context);
            }

            throw new InvalidOperationException($"No proxy was configured for contract '{type.Name}'. An explicit NoProxy() command must be issued thru the Fluent Config Api if a non proxied service instance is required.");
        }

    }

 

}
