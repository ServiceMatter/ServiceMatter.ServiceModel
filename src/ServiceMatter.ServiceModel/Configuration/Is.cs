using System;
using Castle.DynamicProxy;
using NSubstitute;

namespace ServiceMatter.ServiceModel.Configuration
{

    public static class A<T>
    {
        public static T X 
        {
            get
            {
                var type = typeof(T);
                if (type.IsValueType)
                    return default(T);

                if (!type.IsSealed)
                    return (T)Substitute.For(new[] { type },new object[] { });

                return default(T);

            }
        }
    }

    public static class Is
    {
        private static readonly ProxyGenerator _pg = new ProxyGenerator();

        public static T A<T>() 
        {
            var type = typeof(T);
            if (type.IsValueType)
                return default(T);

            if (!type.IsSealed)
                return (T)Instance(type);

            return default(T);
        }

        private static object Instance(Type type)
        {
            if (type.IsClass)
            {
                return null;
            } else
            {
                return _pg.CreateInterfaceProxyWithoutTarget(type);
            }
        }

    }

    /// <summary>
    /// Temprory 
    /// </summary>
    public static class Stub
    {
        private static readonly ProxyGenerator _pg = new ProxyGenerator();

        public static T For<T>() where T : class
        {
            return Substitute.For<T>();
        }

    }

    public static class Stub<T>
        where T : class
    {
        private static readonly ProxyGenerator _pg = new ProxyGenerator();

        public static T For
        {
            get
            {
                return Substitute.For<T>(); //TODO use Castle Dynamic proxy (or NSub or Moq) to return stub
            }
        }

    }
}
