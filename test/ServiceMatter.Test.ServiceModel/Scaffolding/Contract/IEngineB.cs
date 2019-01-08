using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IEngineB
    {
        Task<OperationAResultDto> OperationAaAsync(OperationARequestDto request);
        Task<OperationBResultDto> OperationBbAsync(OperationBRequestDto request);
    }



}
