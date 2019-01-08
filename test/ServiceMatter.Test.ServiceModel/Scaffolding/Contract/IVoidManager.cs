namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public interface IVoidManager
    {
        void NoArgs();
        void OneArg(ArgOne a1);
        void TwoArgs(ArgOne a1, ArgTwo a2);
        void ThreeArgs(ArgOne a1, ArgTwo a2, ArgThree a3);
        void FourArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        void FiveArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        void SixArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);

    }



}
