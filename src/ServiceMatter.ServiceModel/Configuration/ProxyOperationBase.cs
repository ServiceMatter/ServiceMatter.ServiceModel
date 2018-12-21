using System;

namespace ServiceMatter.ServiceModel.Configuration
{
    public class ProxyOperationBase<IContract, TAmbientContext>
         where IContract : class where TAmbientContext : class
    {
        protected readonly ProxyContractBehavior<IContract, TAmbientContext> _contract;
        protected string _operationName { get; }
        protected Type _operationType { get; }

        public ProxyOperationBase(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType)
        {
            _contract = contract;
            _operationName = operationName;
            _operationType = operationType;
        }

        public ProxyContractBehavior<IContract, TAmbientContext> Contract()
        {
            return _contract;
        }
        public ProxyFactoryConfiguration<TAmbientContext> ProxyConfiguration()
        {
            return _contract.ProxyConfiguration();
        }


    }



}
