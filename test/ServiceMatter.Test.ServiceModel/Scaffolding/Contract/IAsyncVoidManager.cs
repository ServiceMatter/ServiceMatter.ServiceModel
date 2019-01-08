using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IAsyncVoidManager
    {
        Task NoArgs();
        Task OnArg(ArgOne a1);
        Task TwoArgs(ArgOne a1, ArgTwo a2);
        Task ThreeArgs(ArgOne a1, ArgTwo a2, ArgThree a3);
        Task FourArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        Task FiveArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        Task SixArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);

    }



}
