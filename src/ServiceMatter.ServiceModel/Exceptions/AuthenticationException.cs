using System;

namespace ServiceMatter.ServiceModel.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base() { }

        public AuthenticationException(string reason) : base(reason) { }
    }
}
