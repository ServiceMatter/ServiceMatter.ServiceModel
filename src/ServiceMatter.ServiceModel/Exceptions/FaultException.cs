using System;

namespace ServiceMatter.ServiceModel.Exceptions
{
    public class FaultException : Exception
    {
        public FaultException(Exception innerEx) : base("Fault: " + innerEx.Message,innerEx) { }
    }
}
