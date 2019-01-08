using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;
using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Proxy
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

        public Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request)
        {
            return InvokeAsync(Service.OperationBbAsync, request);
        }

     
    }
}
