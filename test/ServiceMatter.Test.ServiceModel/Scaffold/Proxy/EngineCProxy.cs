using Service.Matter.Test.ServiceModel.Scaffold.Contract;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;

namespace Service.Matter.Test.ServiceModel.Scaffold.Proxy
{
    public class EngineCProxy<TContext> : ProxyBase<IEngineC, TContext>, IEngineC
       where TContext : class
    {
        public EngineCProxy(IEngineC service, TContext context, ProxyContractBehavior<IEngineC, TContext> behavior) : base(service, context, behavior)
        {
        }

        public OperationAResultDto OperationAa(OperationARequestDto request)
        {
            return Invoke(Service.OperationAa, request);
        }

        public OperationBResultDto OperationBb(OperationBRequestDto request)
        {
            return Invoke(Service.OperationBb, request);
        }

     
    }
}
