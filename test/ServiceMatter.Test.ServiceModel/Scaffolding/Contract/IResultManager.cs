namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IResultManager
    {
        NoArgsResult NoArgs();
        OneArgsResult OnArg(ArgOne a1);
        TwoArgsResult TwoArgs(ArgOne a1, ArgTwo a2);
        TthreeArgsResult ThreeArgs(ArgOne a1, ArgTwo a2, ArgThree a3);
        FourArgsResult FourArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        FiveArgsResult FiveArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        SixArgsResult SixArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
