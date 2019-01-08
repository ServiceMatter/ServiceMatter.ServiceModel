using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using Service.Matter.Test.ServiceModel.Scaffolding.Proxy;
using Service.Matter.Test.ServiceModel.Scaffolding.Service;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;
using ServiceMatter.ServiceModel.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Host
{
    public class ScaffoldingServiceFactory : ServiceFactoryBase<string>
    {
        public ScaffoldingServiceFactory(string context) : base(context)
        {
        }

        public ScaffoldingServiceFactory(IAmbientContextProvider<string> contextProvider) : base(contextProvider)
        {
        }

        public ScaffoldingServiceFactory(string context, ProxyFactoryConfiguration<string> proxyFactoryConfiguration) : base(context, proxyFactoryConfiguration)
        {
        }

        public ScaffoldingServiceFactory(IAmbientContextProvider<string> contextProvider, ProxyFactoryConfiguration<string> proxyFactoryConfiguration) : base(contextProvider, proxyFactoryConfiguration)
        {
        }

        public override IContract Create<IContract>()
        {
            var contract = typeof(IContract);

            if (contract == typeof(IEngineA))
            {
                var service = new EngineAService<string>(Context, this);

                var proxy = ProxyFactory.CreateProxy(service as IContract, Context);

                return proxy as IContract;
            }

            if (contract == typeof(IEngineB))
            {
                var service = new EngineBService<string>(Context, this);

                var proxy = ProxyFactory.CreateProxy(service as IContract, Context);

                return proxy as IContract;
            }

            if (contract == typeof(IEngineC))
            {
                var service = new EngineCService<string>(Context, this);

                var proxy = ProxyFactory.CreateProxy(service as IContract, Context);

                return proxy as IContract;
            }

            throw new InvalidOperationException($"Request for unknown service contract: '{contract.AssemblyQualifiedName}'");
        }
    }
}
