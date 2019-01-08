using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;
using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Service
{
    public class EngineCService<TContext> : ServiceBase<TContext>, IEngineC
       where TContext : class
    {
        public EngineCService(TContext context, ServiceFactoryBase<TContext> factory) : base(context, factory)
        {
        }

        public OperationAResultDto OperationAa(OperationARequestDto request)
        {
            return new OperationAResultDto
            {
                Out = request.In,
            };
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
