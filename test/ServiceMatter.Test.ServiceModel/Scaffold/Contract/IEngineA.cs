using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffold.Contract
{
    public interface  IEngineA
    {

        OperationAResultDto OperationAa(OperationARequestDto request);
        OperationBResultDto OperationBb(OperationBRequestDto request);
    }

    public interface IEngineB
    {

        Task<OperationAResultDto> OperationAaAsync(OperationARequestDto request);
        Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request);
    }

}
