using Service.Matter.Test.ServiceModel.Scaffold.Contract;
using ServiceMatter.ServiceModel;

namespace Service.Matter.Test.ServiceModel.Scaffold.Service
{
    public class EngineAService<TContext> : ServiceBase<TContext>, IEngineA
        where TContext : class
    {
        public EngineAService(TContext context, ServiceFactoryBase<TContext> factory) : base(context, factory)
        {
        }

        public OperationAResultDto OperationAa(OperationARequestDto request)
        {
            return new OperationAResultDto { };
        }

        public OperationBResultDto OperationBb(OperationBRequestDto request)
        {
            return new OperationBResultDto { };
        }
    }
}
