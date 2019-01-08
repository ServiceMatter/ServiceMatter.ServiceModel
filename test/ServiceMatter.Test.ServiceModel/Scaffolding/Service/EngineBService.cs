using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;
using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Service
{
    public class EngineBService<TContext> : ServiceBase<TContext>, IEngineB
      where TContext : class
    {
        public EngineBService(TContext context, ServiceFactoryBase<TContext> factory) : base(context, factory)
        {
        }


        public Task<OperationAResultDto> OperationAaAsync(OperationARequestDto request)
        {
            return Task.FromResult(new OperationAResultDto
            {
                Out = request.In,
            });
        }


        public Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request)
        {
            return Task.FromResult(new OperationBResultDto
            {
                Out = request.In,
            });
        }

    }
}
