namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IOverloadedResultManager
    {
        NoArgsResult Overloaded();
        OneArgsResult Overloaded(ArgOne a1);
        TwoArgsResult Overloaded(ArgOne a1, ArgTwo a2);
        TthreeArgsResult Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3);
        FourArgsResult Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        FiveArgsResult Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        SixArgsResult Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
