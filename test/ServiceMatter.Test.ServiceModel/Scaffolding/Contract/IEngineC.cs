using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IEngineC
    {
        OperationAResultDto OperationAa(OperationARequestDto request);
        Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request);
    }



}
