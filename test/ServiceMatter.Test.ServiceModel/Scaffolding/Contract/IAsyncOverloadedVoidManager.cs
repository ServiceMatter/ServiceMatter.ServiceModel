using System.Threading.Tasks;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IAsyncOverloadedVoidManager
    {
        Task Overloaded();
        Task Overloaded(ArgOne a1);
        Task Overloaded(ArgOne a1, ArgTwo a2);
        Task Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3);
        Task  Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        Task Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        Task Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
