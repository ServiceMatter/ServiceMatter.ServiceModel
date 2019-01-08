using System.Threading.Tasks;
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

        public OperationBResultDto OperationBb(OperationBRequestDto request)
        {
            return new OperationBResultDto
            {
                Out = request.In,
            };
        }

      
    }
}
