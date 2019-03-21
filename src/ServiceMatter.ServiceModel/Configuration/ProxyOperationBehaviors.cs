using ServiceMatter.ServiceModel.Delegates;
using ServiceMatter.ServiceModel.Exceptions;
using ServiceMatter.ServiceModel.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceMatter.ServiceModel.Configuration
{
    #region Action behaviors
    public class ProxyActionBehavior<IContract, TAmbientContext> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        private ActionWrapper<TAmbientContext> _outerWrapper;
        private ActionWrapper<TAmbientContext> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }

                _outerWrapper = (c, a) => a();

                var interceptors = OnIntercept?.GetInvocationList().Cast<ActionWrapper<TAmbientContext>>().ToArray()
                        ?? new ActionWrapper<TAmbientContext>[] { };

                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        private PipelineEventHandler<TAmbientContext> OnAuthenticate;
        private PipelineEventHandler<TAmbientContext> OnAuthorize;
        private PipelineEventHandler<TAmbientContext> OnPreInvoke;
        private PipelineEventHandler<TAmbientContext> OnPostInvoke;
        private ActionWrapper<TAmbientContext> OnIntercept;

        public ProxyActionBehavior(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        #region Fluent Config Api

        public ProxyActionBehavior<IContract, TAmbientContext> AddAuthentication(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext> AddAuthorization(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext> AddPreInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext> AddPostInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        public ProxyActionBehavior<IContract, TAmbientContext> AddInterceptor(ActionWrapper<TAmbientContext> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        #endregion

        #region Internal Pipeline Workers
        internal void Authenticate(TAmbientContext context)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void Authorize(TAmbientContext context)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void PreInvoke(TAmbientContext context)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void PostInvoke(TAmbientContext context)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void Execute(TAmbientContext context, Action action)
        {
            OuterWrapper(context, action);
        }

        #endregion

        #region Private Helpers

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthentication?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthorization?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }
        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnPreInvoke?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }
        private bool VerifyPostInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnPostInvoke?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }
        private ActionWrapper<TAmbientContext> Wrap(ActionWrapper<TAmbientContext> action, ActionWrapper<TAmbientContext> wrapper)
        {
            void wrapped(TAmbientContext c, Action a) => wrapper(c, () => action(c, a));

            return wrapped;
        }

        #endregion
    }

    public class ProxyActionBehavior<IContract, TAmbientContext, T1> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyActionBehavior(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1> OnPreInvoke;
        internal PipelineEventHandler<TAmbientContext, T1> OnPostInvoke;
        internal ActionWrapper<TAmbientContext, T1> OnIntercept;

        private ActionWrapper<TAmbientContext, T1> _outerWrapper;

        private ActionWrapper<TAmbientContext, T1> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1) => a(p1);

                var interceptors = OnIntercept?.GetInvocationList().Cast<ActionWrapper<TAmbientContext, T1>>().ToArray()
                        ?? new ActionWrapper<TAmbientContext, T1>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyActionBehavior<IContract, TAmbientContext, T1> AddAuthentication(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1> AddAuthorization(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1> AddPostInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        public ProxyActionBehavior<IContract, TAmbientContext, T1> AddInterceptor(ActionWrapper<TAmbientContext, T1> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, null, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Execute(TAmbientContext context, Action<T1> action, T1 a1)
        {
            OuterWrapper(context, action, a1);
        }

        private ActionWrapper<TAmbientContext, T1> Wrap(ActionWrapper<TAmbientContext, T1> action, ActionWrapper<TAmbientContext, T1> wrapper)
        {
            ActionWrapper<TAmbientContext, T1> wrapped = (c, a, p1) => wrapper(c, x1 => action(c, a, x1), p1);

            return wrapped;
        }

    }

    public class ProxyActionBehavior<IContract, TAmbientContext, T1, T2> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyActionBehavior(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnPreInvoke;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnPostInvoke;
        internal ActionWrapper<TAmbientContext, T1, T2> OnIntercept;

        private ActionWrapper<TAmbientContext, T1, T2> _outerWrapper;

        private ActionWrapper<TAmbientContext, T1, T2> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1, p2) => a(p1, p2);

                var interceptors = OnIntercept?.GetInvocationList().Cast<ActionWrapper<TAmbientContext, T1, T2>>().ToArray()
                        ?? new ActionWrapper<TAmbientContext, T1, T2>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyActionBehavior<IContract, TAmbientContext, T1, T2> AddAuthentication(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1, T2> AddAuthorization(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1, T2> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehavior<IContract, TAmbientContext, T1, T2> AddPostInvoke(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        public ProxyActionBehavior<IContract, TAmbientContext, T1, T2> AddInterceptor(ActionWrapper<TAmbientContext, T1, T2> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, null, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Execute(TAmbientContext context, Action<T1, T2> action, T1 a1, T2 a2)
        {
            OuterWrapper(context, action, a1, a2);
        }

        private ActionWrapper<TAmbientContext, T1, T2> Wrap(ActionWrapper<TAmbientContext, T1, T2> action, ActionWrapper<TAmbientContext, T1, T2> wrapper)
        {
            ActionWrapper<TAmbientContext, T1, T2> wrapped = (c, a, p1, p2) => wrapper(c, (x1, x2) => action(c, a, x1, x2), p1, p2);

            return wrapped;
        }

    }

    //TODO overloads met 2 t/m 6 params
    #endregion

    #region Function behaviors

    public class ProxyFunctionBehavior<IContract, TAmbientContext, TResult> : ProxyOperationBase<IContract, TAmbientContext>
    where IContract : class where TAmbientContext : class
    {
        public ProxyFunctionBehavior(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        private FunctionWrapper<TAmbientContext, TResult> _outerWrapper;

        internal PipelineEventHandler<TAmbientContext> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext, TResult> OnPostInvoke;
        internal FunctionWrapper<TAmbientContext, TResult> OnIntercept;

        private FunctionWrapper<TAmbientContext, TResult> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a) => a();

                var interceptors = OnIntercept?.GetInvocationList().Cast<FunctionWrapper<TAmbientContext, TResult>>().ToArray()
                        ?? new FunctionWrapper<TAmbientContext, TResult>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> AddAuthentication(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> AddAuthorization(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> AddPreInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> AddPostInvoke(PipelineResultEventHandler<TAmbientContext, TResult> handler)
        {
            OnPostInvoke += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, TResult> AddInterceptor(FunctionWrapper<TAmbientContext, TResult> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void Authorize(TAmbientContext context)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void PreInvoke(TAmbientContext context)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }
        internal void PostInvoke(TAmbientContext context, TResult result)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, result);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, result);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }


        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineResultEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal TResult Execute(TAmbientContext context, Func<TResult> function)
        {
            return OuterWrapper(context, function);
        }

        private FunctionWrapper<TAmbientContext, TResult> Wrap(FunctionWrapper<TAmbientContext, TResult> function, FunctionWrapper<TAmbientContext, TResult> wrapper)
        {
            FunctionWrapper<TAmbientContext, TResult> wrapped = (c, a) => wrapper(c, () => function(c, a));

            return wrapped;
        }



    }

    public class ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyFunctionBehavior(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext, TResult, T1> OnPostInvoke;
        internal FunctionWrapper<TAmbientContext, TResult, T1> OnIntercept;

        private FunctionWrapper<TAmbientContext, TResult, T1> _outerWrapper;

        private FunctionWrapper<TAmbientContext, TResult, T1> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1) => a(p1);

                var interceptors = OnIntercept?.GetInvocationList().Cast<FunctionWrapper<TAmbientContext, TResult, T1>>().ToArray()
                        ?? new FunctionWrapper<TAmbientContext, TResult, T1>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> AddAuthentication(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> AddAuthorization(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> AddPostInvoke(PipelineResultEventHandler<TAmbientContext, TResult, T1> handler)
        {
            OnPostInvoke += handler;
            return this;
        }
        public ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> AddInterceptor(FunctionWrapper<TAmbientContext, TResult, T1> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandlers))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            if (contractHandlers != null)
            {
                foreach (var handler in contractHandlers)
                {
                    handler(context, a1);
                }
            }

            if (operationHandlers != null)
            {
                foreach (var handler in operationHandlers)
                {
                    handler(context, a1);
                }
            }
           
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1)
        {
            //TODO cache verification result and handlers
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, TResult r, T1 a1)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineResultEventHandler<TAmbientContext, TResult, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext, TResult, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal TResult Execute(TAmbientContext context, Func<T1, TResult> function, T1 a1)
        {
            return OuterWrapper(context, function, a1);
        }

        private FunctionWrapper<TAmbientContext, TResult, T1> Wrap(FunctionWrapper<TAmbientContext, TResult, T1> function, FunctionWrapper<TAmbientContext, TResult, T1> wrapper)
        {
            FunctionWrapper<TAmbientContext, TResult, T1> wrapped = (c, a, p1) => wrapper(c, x1 => function(c, a, x1), p1);

            return wrapped;
        }

    }

    //TODO overloads met 2 t/m 6 params

    #endregion

    #region Async action behaviors

    #region Action behaviors
    public class ProxyActionBehaviorAsync<IContract, TAmbientContext> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {

        public ProxyActionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext> OnPreInvoke;
        internal PipelineEventHandler<TAmbientContext> OnPostInvoke;
        internal AsyncActionWrapper<TAmbientContext> OnIntercept;

        private AsyncActionWrapper<TAmbientContext> _outerWrapper;

        private AsyncActionWrapper<TAmbientContext> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }

                _outerWrapper = (c, a) => a(); //returns a task

                var interceptors = OnIntercept?.GetInvocationList().Cast<AsyncActionWrapper<TAmbientContext>>().ToArray()
                        ?? new AsyncActionWrapper<TAmbientContext>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }


        public ProxyActionBehaviorAsync<IContract, TAmbientContext> AddAuthentication(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext> AddAuthorization(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext> AddPreInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext> AddPostInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        internal void Authenticate(TAmbientContext context)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthentication?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthorization?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnPreInvoke?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        private bool VerifyPostInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.OnPostInvoke?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal Task Execute(TAmbientContext context, Func<Task> action)
        {
            return OuterWrapper(context, action);
        }

        private AsyncActionWrapper<TAmbientContext> Wrap(AsyncActionWrapper<TAmbientContext> action, AsyncActionWrapper<TAmbientContext> wrapper)
        {
            AsyncActionWrapper<TAmbientContext> wrapped = (c, a) => wrapper(c, () => action(c, a));

            return wrapped;
        }
    }

    public class ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyActionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1> OnPreInvoke;
        internal PipelineEventHandler<TAmbientContext, T1> OnPostInvoke;
        internal ActionWrapper<TAmbientContext, T1> OnIntercept;

        private ActionWrapper<TAmbientContext, T1> _outerWrapper;

        private ActionWrapper<TAmbientContext, T1> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1) => a(p1);

                var interceptors = OnIntercept?.GetInvocationList().Cast<ActionWrapper<TAmbientContext, T1>>().ToArray()
                        ?? new ActionWrapper<TAmbientContext, T1>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> AddAuthentication(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> AddAuthorization(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> AddPostInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> AddInterceptor(ActionWrapper<TAmbientContext, T1> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, null, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal async Task Execute(TAmbientContext context, Func<T1, Task> asyncAction, T1 a1)
        {
            await asyncAction(a1);
        }

        private ActionWrapper<TAmbientContext, T1> Wrap(ActionWrapper<TAmbientContext, T1> action, ActionWrapper<TAmbientContext, T1> wrapper)
        {
            ActionWrapper<TAmbientContext, T1> wrapped = (c, a, p1) => wrapper(c, x1 => action(c, a, x1), p1);

            return wrapped;
        }

    }

    public class ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyActionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnPreInvoke;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnPostInvoke;
        internal ActionWrapper<TAmbientContext, T1, T2> OnIntercept;

        private ActionWrapper<TAmbientContext, T1, T2> _outerWrapper;

        private ActionWrapper<TAmbientContext, T1, T2> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1, p2) => a(p1, p2);

                var interceptors = OnIntercept?.GetInvocationList().Cast<ActionWrapper<TAmbientContext, T1, T2>>().ToArray()
                        ?? new ActionWrapper<TAmbientContext, T1, T2>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> AddAuthentication(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> AddAuthorization(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> AddPostInvoke(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnPostInvoke += handler;
            return this;
        }

        public ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> AddInterceptor(ActionWrapper<TAmbientContext, T1, T2> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, null, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal async Task Execute(TAmbientContext context, Func<T1, T2, Task> asyncAction, T1 a1, T2 a2)
        {
            await asyncAction(a1, a2);
        }

        private ActionWrapper<TAmbientContext, T1, T2> Wrap(ActionWrapper<TAmbientContext, T1, T2> action, ActionWrapper<TAmbientContext, T1, T2> wrapper)
        {
            ActionWrapper<TAmbientContext, T1, T2> wrapped = (c, a, p1, p2) => wrapper(c, (x1, x2) => action(c, a, x1, x2), p1, p2);

            return wrapped;
        }

    }

    //TODO overloads met 2 t/m 6 params
    #endregion

    #endregion

    #region Async Function behaviors

    public class ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> : ProxyOperationBase<IContract, TAmbientContext>
    where IContract : class where TAmbientContext : class
    {
        public ProxyFunctionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        private AsyncFunctionWrapper<TAmbientContext, TResult> _outerWrapper;

        internal PipelineEventHandler<TAmbientContext> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext, TResult> OnPostInvoke;
        internal AsyncFunctionWrapper<TAmbientContext, TResult> OnIntercept;

        private AsyncFunctionWrapper<TAmbientContext, TResult> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a) => a();

                var interceptors = OnIntercept?.GetInvocationList().Cast<AsyncFunctionWrapper<TAmbientContext, TResult>>().ToArray()
                        ?? new AsyncFunctionWrapper<TAmbientContext, TResult>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> AddAuthentication(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> AddAuthorization(PipelineEventHandler<TAmbientContext> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> AddPreInvoke(PipelineEventHandler<TAmbientContext> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> AddPostInvoke(PipelineResultEventHandler<TAmbientContext, TResult> handler)
        {
            OnPostInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> AddInterceptor(AsyncFunctionWrapper<TAmbientContext, TResult> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void Authorize(TAmbientContext context)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void PreInvoke(TAmbientContext context)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context);
                });
        }

        internal void PostInvoke(TAmbientContext context, TResult result)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, result);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, result);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.AuthenticateHandlers;

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.AuthorizeHandlers;

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineResultEventHandler<TAmbientContext>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal async Task<TResult> Execute(TAmbientContext context, Func<Task<TResult>> function)
        {
            return await OuterWrapper(context, function);
        }

        private AsyncFunctionWrapper<TAmbientContext, TResult> Wrap(AsyncFunctionWrapper<TAmbientContext, TResult> function, AsyncFunctionWrapper<TAmbientContext, TResult> wrapper)
        {
            AsyncFunctionWrapper<TAmbientContext, TResult> wrapped = (c, a) => wrapper(c, async () => await function(c, a));

            return wrapped;
        }

        private FunctionWrapper<TAmbientContext, TResult> Wrap(FunctionWrapper<TAmbientContext, TResult> function, FunctionWrapper<TAmbientContext, TResult> wrapper)
        {
            FunctionWrapper<TAmbientContext, TResult> wrapped = (c, a) => wrapper(c, () => function(c, a));

            return wrapped;
        }


    }

    public class ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> : ProxyOperationBase<IContract, TAmbientContext>
        where IContract : class where TAmbientContext : class
    {
        public ProxyFunctionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext, TResult, T1> OnPostInvoke;
        internal AsyncFunctionWrapper<TAmbientContext, TResult, T1> OnIntercept;

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1> _outerWrapper;

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1) => a(p1);

                var interceptors = OnIntercept?.GetInvocationList().Cast<AsyncFunctionWrapper<TAmbientContext, TResult, T1>>().ToArray()
                        ?? new AsyncFunctionWrapper<TAmbientContext, TResult, T1>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> AddAuthentication(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> AddAuthorization(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> AddPostInvoke(PipelineResultEventHandler<TAmbientContext, TResult, T1> handler)
        {
            OnPostInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> AddInterceptor(AsyncFunctionWrapper<TAmbientContext, TResult, T1> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthentication?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthorization?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, TResult r, T1 a1)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineResultEventHandler<TAmbientContext, TResult, T1>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext, TResult, T1>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }


        internal async Task<TResult> Execute(TAmbientContext context, Func<T1, Task<TResult>> function, T1 a1)
        {
            return await OuterWrapper(context,function,a1);
        }

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1> Wrap(AsyncFunctionWrapper<TAmbientContext, TResult, T1> function, AsyncFunctionWrapper<TAmbientContext, TResult, T1> wrapper)
        {
            AsyncFunctionWrapper<TAmbientContext, TResult, T1> wrapped = (c, a, p1) => wrapper(c, async x1 => await function(c, a, x1), p1);

            return wrapped;
        }


        private FunctionWrapper<TAmbientContext, TResult, T1> Wrap(FunctionWrapper<TAmbientContext, TResult, T1> function, FunctionWrapper<TAmbientContext, TResult, T1> wrapper)
        {
            FunctionWrapper<TAmbientContext, TResult, T1> wrapped = (c, a, p1) => wrapper(c, x1 => function(c, a, x1), p1);

            return wrapped;
        }

    }

    public class ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> : ProxyOperationBase<IContract, TAmbientContext>
    where IContract : class where TAmbientContext : class
    {
        public ProxyFunctionBehaviorAsync(ProxyContractBehavior<IContract, TAmbientContext> contract, string operationName, Type operationType) : base(contract, operationName, operationType)
        {
        }

        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthenticate;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnAuthorize;
        internal PipelineEventHandler<TAmbientContext, T1, T2> OnPreInvoke;
        internal PipelineResultEventHandler<TAmbientContext, TResult, T1, T2> OnPostInvoke;
        internal FunctionWrapper<TAmbientContext, TResult, T1, T2> OnIntercept;

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> _outerWrapper;

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> OuterWrapper
        {
            get
            {
                if (_outerWrapper != null)
                {
                    return _outerWrapper;
                }


                _outerWrapper = (c, a, p1, p2) => a(p1, p2);

                var interceptors = OnIntercept?.GetInvocationList().Cast<AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2>>().ToArray()
                        ?? new AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2>[] { };


                interceptors.DoForEach((wrapper) =>
                {
                    _outerWrapper = Wrap(_outerWrapper, wrapper);

                });

                return _outerWrapper;
            }
        }

        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> AddAuthentication(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthenticate += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> AddAuthorization(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnAuthorize += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> AddPreInvoke(PipelineEventHandler<TAmbientContext, T1, T2> handler)
        {
            OnPreInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> AddPostInvoke(PipelineResultEventHandler<TAmbientContext, TResult, T1, T2> handler)
        {
            OnPostInvoke += handler;
            return this;
        }
        public ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, T2, TResult> AddInterceptor(FunctionWrapper<TAmbientContext, TResult, T1, T2> interceptor)
        {
            OnIntercept += interceptor;
            return this;
        }

        internal void Authenticate(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthenticationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthenticationException($"The authentication for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthenticationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthentication?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthenticate?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void Authorize(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyAuthorizationHandlers(out var contractHandlers, out var operationHandler))
            {
                throw new AuthorizationException($"The Authorization for operation '{_operationName}' on contract '{typeof(IContract).Name}' is not configured.");
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyAuthorizationHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.OnAuthorization?.GetInvocationList().Cast<ContractPipelineEventHandler<TAmbientContext>>().ToArray();

            operationHandlers = OnAuthorize?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PreInvoke(TAmbientContext context, T1 a1, T2 a2)
        {
            if (!VerifyPreInvokeHandlers(out var contractHandlers, out var operationHandlers))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });

            operationHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
        }

        private bool VerifyPreInvokeHandlers(out ContractPipelineEventHandler<TAmbientContext>[] contractHandlers, out PipelineEventHandler<TAmbientContext, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PreInvokeHandlers;

            operationHandlers = OnPreInvoke?.GetInvocationList().Cast<PipelineEventHandler<TAmbientContext, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }

        internal void PostInvoke(TAmbientContext context, TResult r, T1 a1, T2 a2)
        {
            if (!VerifyPostInvokeHandlers(out var contractHandlers, out var operationHandler))
            {
                return;
            }

            contractHandlers?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1, a2);
                });

            operationHandler?
                .DoForEach((handler) =>
                {
                    handler(context, r, a1, a2);
                });
        }

        private bool VerifyPostInvokeHandlers(out PipelineResultEventHandler<TAmbientContext>[] contractHandlers, out PipelineResultEventHandler<TAmbientContext, TResult, T1, T2>[] operationHandlers)
        {
            contractHandlers = _contract.PostInvokeHandlers;

            operationHandlers = OnPostInvoke?.GetInvocationList().Cast<PipelineResultEventHandler<TAmbientContext, TResult, T1, T2>>().ToArray();

            return contractHandlers?.Length > 0 || operationHandlers?.Length > 0;
        }


        internal async Task<TResult> Execute(TAmbientContext context, Func<T1, T2, Task<TResult>> function, T1 a1, T2 a2)
        {
            return await OuterWrapper(context,function,a1,a2);
        }

        private AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> Wrap(AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> function, AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> wrapper)
        {
            AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2> wrappedfunction = (c, f, p1, p2) => wrapper(c, (x1, x2) => function(c, f, x1, x2), p1, p2);

            return wrappedfunction;
        }

    }

    //TODO overloads met 2 t/m 6 params

    #endregion

}
