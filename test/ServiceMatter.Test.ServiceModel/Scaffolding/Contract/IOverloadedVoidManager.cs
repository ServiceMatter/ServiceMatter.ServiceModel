namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{

    public interface IOverloadedVoidManager
    {
        void Overloaded();
        void Overloaded(ArgOne a1);
        void Overloaded(ArgOne a1, ArgTwo a2);
        void Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3);
        void Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4);
        void Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5);
        void Overloaded(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6);
    }



}
