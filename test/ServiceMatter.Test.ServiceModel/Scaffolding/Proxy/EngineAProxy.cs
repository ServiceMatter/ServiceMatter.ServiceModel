using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Proxy
{
    public class EngineAProxy<TContext> : ProxyBase<IEngineA, TContext>, IEngineA
        where TContext : class
    {
        public EngineAProxy(IEngineA service, TContext context, ProxyContractBehavior<IEngineA, TContext> behavior) : base(service, context, behavior)
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

        public OperationCResultDto OperationCc(OperationCRequestDto request)
        {
            return Invoke(Service.OperationCc, request);
        }
    }
}
