using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IAsyncResultManager
    {
        Task<NoArgsResult> NoArgs();
        Task<OneArgsResult> OnArg(ArgOne a1);
        Task<TwoArgsResult> TwoArgs(ArgOne a1, ArgTwo a2);
        Task<TthreeArgsResult> ThreeArgs(ArgOne a1, ArgTwo a2, ArgThree a3);
        Task<FourArgsResult> FourArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        Task<FiveArgsResult> FiveArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        Task<SixArgsResult> SixArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
