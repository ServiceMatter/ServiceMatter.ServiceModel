using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IAsyncOverloadedResultManager
    {
        Task<NoArgsResult> Overloaded();
        Task<OneArgsResult> Overloaded(ArgOne a1);
        Task<TwoArgsResult> Overloaded(ArgOne a1, ArgTwo a2);
        Task<TthreeArgsResult> Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3);
        Task<FourArgsResult> Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        Task<FiveArgsResult> Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        Task<SixArgsResult> Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
