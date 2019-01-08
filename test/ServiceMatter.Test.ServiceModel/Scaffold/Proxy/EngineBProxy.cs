using System.Threading.Tasks;
using Service.Matter.Test.ServiceModel.Scaffold.Contract;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;

namespace Service.Matter.Test.ServiceModel.Scaffold.Proxy
{
    public class EngineBProxy<TContext> : ProxyBase<IEngineB, TContext>, IEngineB
        where TContext : class
    {
        public EngineBProxy(IEngineB service, TContext context, ProxyContractBehavior<IEngineB, TContext> behavior) : base(service, context, behavior)
        {
        }

        public Task<OperationAResultDto> OperationAaAsync(OperationARequestDto request)
        {
            return InvokeAsync(Service.OperationAaAsync, request);
        }

        public Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request)
        {
            return Invoke(Service.OperationBbAsync, request);
        }
    }
}
