using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using ServiceMatter.ServiceModel;
using ServiceMatter.ServiceModel.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Proxy
{
    public class Proxies<TContext> : ProxyBase<IVoidManager, TContext>, IVoidManager
        where TContext : class
    {
        public Proxies(IVoidManager service, TContext context, ProxyContractBehavior<IVoidManager, TContext> behavior) : base(service, context, behavior)
        {
        }


        public void NoArgs()
        {
            Invoke(Service.NoArgs);
        }

        public void OneArg(ArgOne a1)
        {
            Invoke(Service.OneArg, a1);
        }

        public void TwoArgs(ArgOne a1, ArgTwo a2)
        {
            throw new NotImplementedException();
        }

        public void ThreeArgs(ArgOne a1, ArgTwo a2, ArgThree a3)
        {
            throw new NotImplementedException();
        }
        public void FourArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4)
        {
            throw new NotImplementedException();
        }
        public void FiveArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5)
        {
            throw new NotImplementedException();
            //Invoke(Service.FiveArgs, a1, a2, a3, a4, a5);
        }
        public void SixArgs(ArgOne a1, ArgTwo a2, ArgThree a3, ArgFour a4, ArgFive a5, ArgSix a6)
        {
        }



    }
}
