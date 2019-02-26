using System;
using System.Collections.Generic;
using ServiceMatter.ServiceModel.Configuration;

namespace ServiceMatter.ServiceModel.Proxy
{
    public class ProxyFactory<TAmbientContext>
        where TAmbientContext : class
    {
        public ProxyFactory(ProxyFactoryConfiguration<TAmbientContext> config)
        {
            Config = config;
        }

        ProxyFactoryConfiguration<TAmbientContext> Config { get; set; }

        public T CreateProxy<T>(T service, TAmbientContext context) 
            where T : class
        {
            return Config.CreateProxy(service, context);
        }
    }


    public interface IProxyConfigurationHelper<TInterface>
        where TInterface : class
    {

        IProxyConfigurationHelper<TInterface> RegisterAuthenticationHandler();
        IProxyConfigurationHelper<TInterface> RegisterAuthorizationHandler();
        IProxyConfigurationHelper<TInterface> RegisterPreInvokeHandler();
        IProxyConfigurationHelper<TInterface> RegisterPostInvokeHandler();
        IProxyConfigurationHelper<TInterface> RegisterInvokeWrapper<T1,TResult>(Func<T1,TResult> forMember, Func<T1,Func<T1,TResult>,TResult> wrapper);
        IProxyConfigurationHelper<TInterface> RegisterInvokeWrapper<T1, TResult>(Func<T1, Func<T1, TResult>, TResult> wrapper);
        IProxyConfigurationHelper<TInterface> RegisterInvokeWrapper<TResult>(Func<Func<TResult>, TResult> wrapper);
    }


    internal interface IOperationHandlerCollection<TContext>
    {

        #region Handle Actions

        Action<TContext> GetActionHandler(Action forMember, string serviceMemberName);
        Action<TContext, T1> GetActionHandler<T1>(Action<T1> forMember, string serviceMemberName);
        Action<TContext, T1, T2> GetActionHandler<T1, T2>(Action<T1, T2> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3> GetActionHandler<T1, T2, T3>(Action<T1, T2, T3> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4> GetActionHandler<T1, T2, T3, T4>(Action<T1, T2, T3, T4> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5> GetActionHandler<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5, T6> GetActionHandler<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> forMember, string serviceMemberName);

        #endregion

        #region Handle Functions

        Action<TContext> GetFuncHandler<TReturn>(Func<TReturn> forMember, string serviceMemberName);
        Action<TContext, T1> GetFuncHandler<T1, TReturn>(Func<T1, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2> GetFuncHandler<T1, T2, TReturn>(Func<T1, T2, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3> GetFuncHandler<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4> GetFuncHandler<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5> GetFuncHandler<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5, T6> GetFuncHandler<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5, T6, T7> GetFuncHandler<T1, T2, T3, T4, T5, T6, T7, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> forMember, string serviceMemberName);
        Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> GetFuncHandler<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> forMember, string serviceMemberName);

        #endregion

        #region Register Action Handlers

        void Register(Action forMember, Action<TContext> handler);
        void Register<T1>(Action<T1> forMember, Action<TContext, T1> handler);
        void Register<T1, T2>(Action<T1, T2> forMember, Action<TContext, T1, T2> handler);
        void Register<T1, T2, T3>(Action<T1, T2, T3> forMember, Action<TContext, T1, T2, T3> handler);
        void Register<T1, T2, T3, T4>(Action<T1, T2, T3, T4> forMember, Action<TContext, T1, T2, T3, T4> handler);
        void Register<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> forMember, Action<TContext, T1, T2, T3, T4, T5> handler);
        void Register<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> forMember, Action<TContext, T1, T2, T3, T4, T5, T6> handler);
        void Register<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7> handler);
        void Register<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> handler);

        #endregion

        #region Register Function Handlers

        void Register<TReturn>(Func<TReturn> forMember, Action<TContext> handler);
        void Register<T1, TReturn>(Func<T1, TReturn> forMember, Action<TContext, T1> handler);
        void Register<T1, T2, TReturn>(Func<T1, T2, TReturn> forMember, Action<TContext, T1, T2> handler);
        void Register<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> forMember, Action<TContext, T1, T2, T3> handler);
        void Register<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> forMember, Action<TContext, T1, T2, T3, T4> handler);
        void Register<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5> handler);
        void Register<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6> handler);
        void Register<T1, T2, T3, T4, T5, T6, T7, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7> handler);
        void Register<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> handler);

        #endregion

    }

    internal class OperationHandlerCollection<TContext> : IOperationHandlerCollection<TContext>
    {
        private readonly Dictionary<string, Dictionary<Type, object>> _handlers = new Dictionary<string, Dictionary<Type, object>>();

        internal OperationHandlerCollection()
        {

        }

        #region Action Interceptors

        public Action<TContext> GetActionHandler(Action forMember, string serviceMemberName)
        {
            return GetInterceptor<Action, Action<TContext>>(serviceMemberName);
        }

        public Action<TContext, T1> GetActionHandler<T1>(Action<T1> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1>, Action<TContext, T1>>(serviceMemberName);
        }

        public Action<TContext, T1, T2> GetActionHandler<T1, T2>(Action<T1, T2> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2>, Action<TContext, T1, T2>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3> GetActionHandler<T1, T2, T3>(Action<T1, T2, T3> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3>, Action<TContext, T1, T2, T3>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4> GetActionHandler<T1, T2, T3, T4>(Action<T1, T2, T3, T4> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3, T4>, Action<TContext, T1, T2, T3, T4>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5> GetActionHandler<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3, T4, T5>, Action<TContext, T1, T2, T3, T4, T5>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6> GetActionHandler<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3, T4, T5, T6>, Action<TContext, T1, T2, T3, T4, T5, T6>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6, T7> GetActionHandler<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3, T4, T5, T6, T7>, Action<TContext, T1, T2, T3, T4, T5, T6, T7>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> GetActionHandler<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> forMember, string serviceMemberName)
        {
            return GetInterceptor<Action<T1, T2, T3, T4, T5, T6, T7, T8>, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8>>(serviceMemberName);
        }

        #endregion

        #region Function Interceptors

        public Action<TContext> GetFuncHandler<TReturn>(Func<TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<TReturn>, Action<TContext>>(serviceMemberName);
        }

        public Action<TContext, T1> GetFuncHandler<T1, TReturn>(Func<T1, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, TReturn>, Action<TContext, T1>>(serviceMemberName);
        }

        public Action<TContext, T1, T2> GetFuncHandler<T1, T2, TReturn>(Func<T1, T2, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, TReturn>, Action<TContext, T1, T2>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3> GetFuncHandler<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, TReturn>, Action<TContext, T1, T2, T3>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4> GetFuncHandler<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, T4, TReturn>, Action<TContext, T1, T2, T3, T4>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5> GetFuncHandler<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, T4, T5, TReturn>, Action<TContext, T1, T2, T3, T4, T5>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6> GetFuncHandler<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, T4, T5, T6, TReturn>, Action<TContext, T1, T2, T3, T4, T5, T6>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6, T7> GetFuncHandler<T1, T2, T3, T4, T5, T6, T7, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, T4, T5, T6, T7, TReturn>, Action<TContext, T1, T2, T3, T4, T5, T6, T7>>(serviceMemberName);
        }

        public Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> GetFuncHandler<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> forMember, string serviceMemberName)
        {
            return GetInterceptor<Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8>>(serviceMemberName);
        }

        #endregion


        #region Register Action Interceptors

        public void Register(Action forMember, Action<TContext> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1>(Action<T1> forMember, Action<TContext, T1> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2>(Action<T1, T2> forMember, Action<TContext, T1, T2> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3>(Action<T1, T2, T3> forMember, Action<TContext, T1, T2, T3> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4>(Action<T1, T2, T3, T4> forMember, Action<TContext, T1, T2, T3, T4> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> forMember, Action<TContext, T1, T2, T3, T4, T5> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> forMember, Action<TContext, T1, T2, T3, T4, T5, T6> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }

        #endregion

        #region Register Function Interceptors

        public void Register<TReturn>(Func<TReturn> forMember, Action<TContext> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, TReturn>(Func<T1, TReturn> forMember, Action<TContext, T1> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, TReturn>(Func<T1, T2, TReturn> forMember, Action<TContext, T1, T2> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> forMember, Action<TContext, T1, T2, T3> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> forMember, Action<TContext, T1, T2, T3, T4> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6, T7, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }
        public void Register<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> forMember, Action<TContext, T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            RegisterToCollection(forMember, forMember.Method.Name, handler);
        }

        #endregion

        private THandler GetInterceptor<TMember, THandler>(string forMemberName) where TMember : class where THandler : class
        {
            Dictionary<Type, object> memberHandlers = null;
            if (!_handlers.ContainsKey(forMemberName))
            {
                return null;
            }
            memberHandlers = _handlers[forMemberName];
            var t = typeof(TMember);

            if (memberHandlers.ContainsKey(t))
            {
                var retval = memberHandlers[t];
                if (retval is THandler)
                    return (THandler)retval;
            }
            return null;
        }

        private void RegisterToCollection<TMember, THandler>(TMember forMember, string forMemberName, THandler handler)
        {
            Dictionary<Type, object> memberHandlers = null;
            if (!_handlers.ContainsKey(forMemberName))
            {
                memberHandlers = new Dictionary<Type, object>();
                _handlers.Add(forMemberName, memberHandlers);
            }
            else
            {
                memberHandlers = _handlers[forMemberName];
            }
            memberHandlers.Add(typeof(TMember), handler);
        }

    }

}
