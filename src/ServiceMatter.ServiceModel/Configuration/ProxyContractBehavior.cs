using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ServiceMatter.ServiceModel.Delegates;
using ServiceMatter.ServiceModel.Extensions;

namespace ServiceMatter.ServiceModel.Configuration
{
    public class NoProxyBehavior<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        private readonly ProxyFactoryConfiguration<TAmbientContext> _factoryConfig;

        public NoProxyBehavior(ProxyFactoryConfiguration<TAmbientContext> factoryConfig)
        {
            _factoryConfig = factoryConfig;
        }

        public ProxyFactoryConfiguration<TAmbientContext> ProxyConfiguration()
        {
            return _factoryConfig;
        }
    }

    public class ProxyContractBehavior<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        private bool _noProxy = false;
        private ConstructorInfo _constructor = null;

        private readonly IDictionary<string, IDictionary<Type, object>> _operationConfigurations = new Dictionary<string, IDictionary<Type, object>>(10, StringComparer.Ordinal); //TODO consider using lists/arrays in stead of dics because this is faster for collections smaller then ...
        private readonly ProxyFactoryConfiguration<TAmbientContext> _factoryConfig;

        private EventHandler<ErrorEventArgs>[] _errorHandlers;
        private ContractPipelineEventHandler<TAmbientContext>[] _authenticateHandlers;
        private ContractPipelineEventHandler<TAmbientContext>[] _authorizeHandlers;
        private ContractPipelineEventHandler<TAmbientContext>[] _preInvokeHandlers;
        private PipelineResultEventHandler<TAmbientContext>[] _postInvokeHandlers;

        internal EventHandler<ErrorEventArgs> OnError;
        internal ContractPipelineEventHandler<TAmbientContext> OnAuthentication;
        internal ContractPipelineEventHandler<TAmbientContext> OnAuthorization;
        internal ContractPipelineEventHandler<TAmbientContext> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext> OnPostInvoke;

        internal EventHandler<ErrorEventArgs>[] ErrorHandlers
        {
            get
            {
                if (_errorHandlers != null)
                {
                    return _errorHandlers;
                }

                _errorHandlers = OnError?.GetInvocationList().Cast<EventHandler<ErrorEventArgs>>().ToArray()
                    ?? new EventHandler<ErrorEventArgs>[] { };

                return _errorHandlers;
            }
        }
        internal ContractPipelineEventHandler<TAmbientContext>[] AuthenticateHandlers
        {
            get
            {
                if (_authenticateHandlers != null)
                {
                    return _authenticateHandlers;
                }

                _authenticateHandlers = OnAuthentication?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray()
                    ?? new ContractPipelineEventHandler<TAmbientContext>[] { };

                return _authenticateHandlers;
            }
        }
        internal ContractPipelineEventHandler<TAmbientContext>[] AuthorizeHandlers
        {
            get
            {
                if (_authorizeHandlers != null)
                {
                    return _authorizeHandlers;
                }

                _authorizeHandlers = OnAuthorization?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray()
                    ?? new ContractPipelineEventHandler<TAmbientContext>[] { };

                return _authorizeHandlers;
            }
        }
        internal ContractPipelineEventHandler<TAmbientContext>[] PreInvokeHandlers
        {
            get
            {
                if (_preInvokeHandlers != null)
                {
                    return _preInvokeHandlers;
                }

                _preInvokeHandlers = OnPreInvoke?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray()
                    ?? new ContractPipelineEventHandler<TAmbientContext>[] { };

                return _preInvokeHandlers;
            }
        }
        internal PipelineResultEventHandler<TAmbientContext>[] PostInvokeHandlers
        {
            get
            {
                if (_postInvokeHandlers != null)
                {
                    return _postInvokeHandlers;
                }

                _postInvokeHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext>>().ToArray()
                    ?? new PipelineResultEventHandler<TAmbientContext>[] { };

                return _postInvokeHandlers;
            }
        }

        internal ProxyContractBehavior(ProxyFactoryConfiguration<TAmbientContext> factoryConfig)
        {

            Debug.Assert(factoryConfig != null);

            _factoryConfig = factoryConfig;
        }

        #region Fluent Config API

        public ProxyFactoryConfiguration<TAmbientContext> ProxyConfiguration()
        {
            return _factoryConfig;
        }

        public ProxyContractBehavior<IContract, TAmbientContext> Use<T>()
        {

            var type = typeof(T);

            Debug.Assert(!type.IsInterface);
            Debug.Assert(type.IsClass);
            Debug.Assert(typeof(IContract).IsAssignableFrom(type));


            var constructor = type.GetConstructor(new[] { typeof(IContract), typeof(TAmbientContext), typeof(ProxyContractBehavior<IContract, TAmbientContext>) });

            Debug.Assert(constructor != null);

            _constructor = constructor;

            return this;
        }

        private IEnumerable<Type> BuildGenericTypes(MethodInfo method, Type returnType = null)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return parameter.ParameterType;
            }

            if (returnType != null)
            {
                yield return returnType;
            }
        }

        //KEEP
        //KEEP TODO Consider for alternate API method, first solve returning async behavior for async methods
        //KEEP
        public ProxyActionBehavior<IContract, TAmbientContext> ForMethod(Expression<Action<IContract>> expression)
        {
            if (!TryGetOperationInfo(expression, out var methodName, out var operationType))
            {
                throw new ArgumentException($"The Expression Body must be a {nameof(MethodCallExpression)}. E.g: 'x => x.SomeOperation(args)'", nameof(expression));
            };


            var operationConfig = EnsureActionConfig(methodName, operationType);

            return operationConfig;
        }

        public ProxyFunctionBehavior<IContract, TAmbientContext,T1,TResult> ForMethod<T1,TResult>(Expression<Action<IContract>> expression)
        {
            if (!TryGetOperationInfo(expression, out var methodName, out var operationType))
            {
                throw new ArgumentException($"The Expression Body must be a {nameof(MethodCallExpression)}. E.g: 'x => x.SomeOperation(args)'", nameof(expression));
            };

            var operationConfig = EnsureFunctionConfig<T1,TResult>(methodName, operationType);

            return operationConfig;
        }

        //KEEP
        //KEEP
        //KEEP

        #region Function behavior config

        #region Synchronous
        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> ForOperation<TResult>(Func<TResult> function)
        {
            var operationConfig = EnsureFunctionConfig<TResult>(function.Method.Name, function.GetType());

            return operationConfig;
        }

        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> ForOperation<T1, TResult>(Func<T1, TResult> function, T1 a1)
        {
            var operationConfig = EnsureFunctionConfig<T1, TResult>(function.Method.Name, function.GetType());

            return operationConfig;
        }

        #endregion

        #region Asynchronous
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> ForOperation<TResult>(Func<Task<TResult>> function)
        {
            var operationConfig = EnsureAsyncFunctionConfig<TResult>(function.Method.Name, function.GetType());

            return operationConfig;
        }

        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> ForOperation<T1, TResult>(Func<T1,Task<TResult>> function, T1 a1)
        {
            var operationConfig = EnsureAsyncFunctionConfig<T1, TResult>(function.Method.Name, function.GetType());

            return operationConfig;
        }

        #endregion

        private ProxyFunctionBehavior<IContract, TAmbientContext, TResult> EnsureFunctionConfig<TResult>(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyFunctionBehavior<IContract, TAmbientContext, TResult>(this, methodName, type));

            return operationConfig;
        }

        private ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> EnsureFunctionConfig<T1, TResult>(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult>(this, methodName, type));

            return operationConfig;
        }

        private ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> EnsureAsyncFunctionConfig<TResult>(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult>(this, methodName, type));

            return operationConfig;
        }

        private ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> EnsureAsyncFunctionConfig<T1, TResult>(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult>(this, methodName, type));

            return operationConfig;
        }

        #endregion

        #region Action behavior config

        public ProxyActionBehavior<IContract, TAmbientContext> ForOperation(Action action)
        {
            var operationConfig = EnsureActionConfig(action.Method.Name, action.GetType());

            return operationConfig;
        }

        public ProxyActionBehavior<IContract, TAmbientContext, T1> ForOperation<T1>(Action<T1> action, T1 a1)
        {
            var operationConfig = EnsureActionConfig<T1>(action.Method.Name, action.GetType());

            return operationConfig;
        }

        private ProxyActionBehavior<IContract, TAmbientContext> EnsureActionConfig(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyActionBehavior<IContract, TAmbientContext>(this, methodName, type));

            return operationConfig;
        }

        private ProxyActionBehavior<IContract, TAmbientContext, T1> EnsureActionConfig<T1>(string methodName, Type type)
        {
            var operationConfig = EnsureOperationConfig(methodName, type, () => new ProxyActionBehavior<IContract, TAmbientContext, T1>(this, methodName, type));

            return operationConfig;
        }

        #endregion

        private TBehavior EnsureOperationConfig<TBehavior>(string methodName, Type operationType, Func<TBehavior> factory)
            where TBehavior : class
        {
            if (!_operationConfigurations.TryGetValue(methodName, out var overloads))
            {
                overloads = new Dictionary<Type, object>(6);
                _operationConfigurations[methodName] = overloads;
            }

            if (!overloads.TryGetValue(operationType, out var operationConfig))
            {
                operationConfig = factory();
                overloads[operationType] = operationConfig;
            }

            return operationConfig as TBehavior;
        }

        private Type GetGenericType(bool returnsVoid, int nrOfTypeParams)
        {
            if (returnsVoid)
            {
                switch (nrOfTypeParams)
                {
                    case 0: return typeof(Action);
                    case 1: return typeof(Action<>);
                    case 2: return typeof(Action<,>);
                    case 3: return typeof(Action<,,>);
                    case 4: return typeof(Action<,,,>);
                    case 5: return typeof(Action<,,,,>);
                    case 6: return typeof(Action<,,,,,>);
                    default: throw new NotSupportedException("Support is limited to methods with no more then 6 parameters.");
                }
            }

            switch (nrOfTypeParams)
            {
                case 0: return typeof(Func<>);
                case 1: return typeof(Func<,>);
                case 2: return typeof(Func<,,>);
                case 3: return typeof(Func<,,,>);
                case 4: return typeof(Func<,,,,>);
                case 5: return typeof(Func<,,,,,>);
                case 6: return typeof(Func<,,,,,,>);
                default: throw new NotSupportedException("Support is limited to methods with no more then 6 parameters.");
            }

        }

        public NoProxyBehavior<IContract, TAmbientContext> NoProxy()
        {
            _noProxy = true;
            _constructor = null;
            return new NoProxyBehavior<IContract, TAmbientContext>(_factoryConfig);
        }

        public ProxyContractBehavior<IContract, TAmbientContext> AddErrorHandler(EventHandler<ErrorEventArgs> handler)
        {
            OnError += handler;
            return this;
        }
        public ProxyContractBehavior<IContract, TAmbientContext> AddAuthentication(ContractPipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthentication += handler;
            return this;
        }
        public ProxyContractBehavior<IContract, TAmbientContext> AddAuthorization(ContractPipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthorization += handler;
            return this;
        }
        public ProxyContractBehavior<IContract, TAmbientContext> AddPreInvoke(ContractPipelineEventHandler<TAmbientContext> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyContractBehavior<IContract, TAmbientContext> AddPostInvoke(PipelineResultEventHandler<TAmbientContext> handler)
        {
            OnPostInvoke = handler;
            return this;
        }

        #endregion
        internal IContract Create(IContract service, TAmbientContext context)
        {
            if (_noProxy)
            {
                return service;
            }

            return _constructor.Invoke(new object[] { service, context, this }) as IContract;
        }

        #region Operation overloads

        #region Synchronous

        #region Actions
        internal ProxyActionBehavior<IContract, TAmbientContext> Operation(Action action, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Action), out var behavior))
                {
                    return behavior as ProxyActionBehavior<IContract, TAmbientContext>;
                }
            }

            return new ProxyActionBehavior<IContract, TAmbientContext>(this, memberName, typeof(Action));
        }

        internal ProxyActionBehavior<IContract, TAmbientContext, T1> Operation<T1>(Action<T1> action, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Action<T1>), out var behavior))
                {
                    return behavior as ProxyActionBehavior<IContract, TAmbientContext, T1>;
                }
            }

            return new ProxyActionBehavior<IContract, TAmbientContext, T1>(this, memberName, typeof(Action<T1>));
        }

        internal ProxyActionBehavior<IContract, TAmbientContext, T1, T2> Operation<T1, T2>(Action<T1, T2> action, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Action<T1, T2>), out var behavior))
                {
                    return behavior as ProxyActionBehavior<IContract, TAmbientContext, T1, T2>;
                }
            }

            return new ProxyActionBehavior<IContract, TAmbientContext, T1, T2>(this, memberName, typeof(Action<T1, T2>));
        }

        #endregion

        #region Functions
        internal ProxyFunctionBehavior<IContract, TAmbientContext, TResult> Operation<TResult>(Func<TResult> function, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {
                if (overloads.TryGetValue(typeof(Func<TResult>), out var behavior))
                {
                    return behavior as ProxyFunctionBehavior<IContract, TAmbientContext, TResult>;
                }
            }
            return new ProxyFunctionBehavior<IContract, TAmbientContext, TResult>(this, memberName, typeof(Func<TResult>));
        }

        internal ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> Operation<T1, TResult>(Func<T1, TResult> function, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {
                if (overloads.TryGetValue(typeof(Func<T1, TResult>), out var behavior))
                {
                    return behavior as ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult>;
                }
            }

            return new ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult>(this, memberName, typeof(Func<T1, TResult>));
        }
        #endregion
        #endregion

        #region Asynchronous

        #region Actions Async
        internal ProxyActionBehaviorAsync<IContract, TAmbientContext> Operation(Func<Task> asyncAction, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Func<Task>), out var behavior))
                {
                    return behavior as ProxyActionBehaviorAsync<IContract, TAmbientContext>;
                }
            }

            return null;
        }

        internal ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> Operation<T1>(Func<T1, Task> asyncAction, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Func<T1, Task>), out var behavior))
                {
                    return behavior as ProxyActionBehaviorAsync<IContract, TAmbientContext, T1>;
                }
            }

            return null;
            //throw new InvalidOperationException($"Call to unknown operation '{memberName}' with signature '{typeof(Func<T1,Task>).Name}' on contract '{typeof(IContract).FullName}'");
        }

        internal ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> Operation<T1, T2>(Func<T1, T2, Task> asyncAction, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Func<T1, T2, Task>), out var behavior))
                {
                    return behavior as ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2>;
                }
            }

            return null;
        }
        #endregion

        #region Functions Async

        internal ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> Operation<TResult>(Func<Task<TResult>> asyncAction, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Func<Task<TResult>>), out var behavior))
                {
                    return behavior as ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult>;
                }
            }

            return null;
        }

        internal ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> Operation<T1, TResult>(Func<T1, Task<TResult>> asyncAction, string memberName)
        {
            if (_operationConfigurations.TryGetValue(memberName, out var overloads))
            {

                if (overloads.TryGetValue(typeof(Func<T1, Task<TResult>>), out var behavior))
                {
                    return behavior as ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult>;
                }
            }

            return null;
        }

        #endregion

        #endregion

        #endregion

        #region Error Behavior
        internal void RaiseError(string source, TAmbientContext context, Exception exception, params object[] input)
        {

            var sender = this;
            var args = new ErrorEventArgs
            {
                Context = context,
                Exception = exception,
                Input = input,
                Source = source
            };

            ErrorHandlers.DoForEach((handler) =>
            {
                handler(sender, args);
            });

        }
        #endregion

        /// <summary>
        /// KEEP THIS METHOD
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="methodName"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private bool TryGetOperationInfo(Expression<Action<IContract>> expression, out string methodName, out Type operationType)
        {
            methodName = null;
            operationType = null;

            var methodCall = expression.Body as MethodCallExpression;
            var method = methodCall?.Method;

            if (method == null)
            {
                return false;
            }

            Type returnType = method.ReturnType == typeof(void) ? null : method.ReturnType;
            var typeParameters = method.GetParameters().Select(x => x.ParameterType).ToList();
            Type genericType = GetGenericType(returnType == null, typeParameters.Count);

            if (returnType != null)
            {
                typeParameters.Add(returnType);
            }

            operationType = genericType.MakeGenericType(typeParameters.ToArray());

            methodName = method.Name;

            return true;
        }

        [DataContract]
        public class ErrorEventArgs
        {
            [DataMember]
            public TAmbientContext Context { get; set; }
            [DataMember]
            public object[] Input { get; set; }
            [DataMember]
            public Exception Exception { get; set; }
            [DataMember]
            public string Source { get; set; }
        }

    }


}
