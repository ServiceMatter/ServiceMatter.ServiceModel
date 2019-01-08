using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Service
{
    public class EngineAService<TContext> : ServiceBase<TContext>, IEngineA
        where TContext : class
    {
        public EngineAService(TContext context, ServiceFactoryBase<TContext> factory) : base(context, factory)
        {
        }

        public OperationAResultDto OperationAa(OperationARequestDto request)
        {
            return new OperationAResultDto
            {
                Out = request.In,
            };
        }

        public OperationBResultDto OperationBb(OperationBRequestDto request)
        {
            return new OperationBResultDto
            {
                Out = request.In,
            };
        }

        public OperationCResultDto OperationCc(OperationCRequestDto request)
        {
            return new OperationCResultDto
            {
                Out = request.In,
            };
        }
    }
}
