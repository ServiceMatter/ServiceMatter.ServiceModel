using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ServiceMatter.ServiceModel.Configuration;
using ServiceMatter.ServiceModel.Exceptions;
using ServiceMatter.ServiceModel.Extensions;

namespace ServiceMatter.ServiceModel
{

    public abstract class ProxyBase<IContract, TAmbientContext> where IContract : class where TAmbientContext : class
    {
        private ProxyContractBehavior<IContract, TAmbientContext> ContractBehavior { get; }

        protected IContract Service { get; }

        protected ProxyBase(IContract service, TAmbientContext context, ProxyContractBehavior<IContract, TAmbientContext> behavior)
        {
            Debug.Assert(service != null);
            Debug.Assert(behavior != null);

            Service = service;
            Context = context;
            ContractBehavior = behavior;
        }

        protected virtual TAmbientContext Context { get; }

        #region ExecutePipeline overloads

        private void ExecutePipeline(Action authN, Action authZ, Action preInvoke, Action innerInvoke, Action postInvoke)
        {
            authN();
            authZ();
            preInvoke();
            innerInvoke();
            postInvoke();
        }

        private async Task ExecutePipelineAsync(Action authN, Action authZ, Action preInvoke, Func<Task> innerInvoke, Action postInvoke)
        {
            authN();
            authZ();
            preInvoke();
            await innerInvoke();
            postInvoke();
        }

        private TResult ExecutePipelineWithResult<TResult>(Action authN, Action authZ, Action preInvoke, Func<TResult> innerInvoke, Action<TResult> postInvoke)
        {
            authN();
            authZ();
            preInvoke();
            var result = innerInvoke();
            postInvoke(result);

            return result;
        }

        private async Task<TResult> ExecutePipelineWithResultAsync<TResult>(Action authN, Action authZ, Action preInvoke, Func<Task<TResult>> funcAsync, Action<TResult> postInvoke)
        {
            authN();
            authZ();
            preInvoke();
            var result = await funcAsync();
            postInvoke(result);

            return result;
        }

        #endregion

        #region Synchronous 

        #region Invoke

        #region Invoke Actions
        protected void Invoke(Action action, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(action, serviceMemberName);

            ExecutePipeline(
                () => AuthenticateInvoke(operationBehaviour, Context),
                () => AuthorizeInvoke(operationBehaviour, Context),
                () => PreInvoke(operationBehaviour, Context),
                () => operationBehaviour.Execute(Context, action),
                () => PostInvoke(operationBehaviour, Context)
                );
        }

        protected void Invoke<T1>(T1 a1, Action<T1> action, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(action, serviceMemberName);

            ExecutePipeline(
                () => AuthenticateInvoke(operationBehaviour, Context, a1),
                () => AuthorizeInvoke(operationBehaviour, Context, a1),
                () => PreInvoke(operationBehaviour, Context, a1),
                () => operationBehaviour.Execute(Context, action, a1),
                () => PostInvoke(operationBehaviour, Context, a1)
                );

        }
        #endregion

        #region Invoke Functions

        protected TResult Invoke<TResult>(Func<TResult> function, [CallerMemberName] string memberName = null)
        {
            var operationBehavior = ContractBehavior.Operation(function, memberName);

            return ExecutePipelineWithResult(
                () => AuthenticateInvoke(operationBehavior, Context),
                () => AuthorizeInvoke(operationBehavior, Context),
                () => PreInvoke(operationBehavior, Context),
                () => InnerInvoke(operationBehavior, Context, function),
                (result) => PostInvoke(operationBehavior, Context, result)
            );
        }

        protected TResult Invoke<T1, TResult>(Func<T1, TResult> function, T1 a1, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(function, serviceMemberName);

            return ExecutePipelineWithResult(
                () => AuthenticateInvoke(operationBehaviour, Context, a1),
                () => AuthorizeInvoke(operationBehaviour, Context, a1),
                () => PreInvoke(operationBehaviour, Context, a1),
                () => operationBehaviour.Execute(Context, function, a1),
                (result) => PostInvoke(operationBehaviour, Context, result, a1)
                );
        }

        #endregion

        #endregion

        #region Authenticate

        #region Authenticate Actions

        private void AuthenticateInvoke(ProxyActionBehavior<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthenticateInvoke<T1>(ProxyActionBehavior<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        private void AuthenticateInvoke<T1, T2>(ProxyActionBehavior<IContract, TAmbientContext, T1, T2> operationBehavior, TAmbientContext context, T1 a1, T2 a2)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1, a2);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
            }

        }
        #endregion

        #region Authenticate Functions

        private void AuthenticateInvoke<TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthenticateInvoke<T1, TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }
        }
        #endregion

        #endregion

        #region Authorize

        #region Authorize Actions

        private void AuthorizeInvoke(ProxyActionBehavior<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthorizeInvoke<T1>(ProxyActionBehavior<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context, a1);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        #endregion

        #region Authorize Functions

        private void AuthorizeInvoke<TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthorizeInvoke<T1, TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context, a1);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        #endregion

        #endregion

        #region PreInvoke

        #region Actions
        private void PreInvoke(ProxyActionBehavior<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void PreInvoke<T1>(ProxyActionBehavior<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }
        #endregion

        #region Functions
        private void PreInvoke<TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void PreInvoke<T1, TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }
        #endregion

        #endregion

        #region InnerInvoke

        #region InnerInvoke Actions
        private void InnerInvoke(ProxyActionBehavior<IContract, TAmbientContext> operationBehavior, TAmbientContext context, Action action)
        {
            try
            {
                if (operationBehavior != null)
                {
                    operationBehavior.Execute(context, action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e);
                throw new FaultException(e);
            }

        }

        private void InnerInvoke<T1>(ProxyActionBehavior<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, Action<T1> action, T1 a1)
        {

            try
            {
                if (operationBehavior != null)
                {
                    operationBehavior.Execute(context, action, a1);
                }
                else
                {
                    action(a1);
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e, a1);
                throw new FaultException(e);
            }

        }

        #endregion

        #region InnerInvoke Functions

        private TResult InnerInvoke<TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context, Func<TResult> func)
        {
            try
            {
                if (operationBehavior != null)
                {
                    return operationBehavior.Execute(context, func);
                }
                else
                {
                    return func();
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e);
                throw new FaultException(e);
            }

        }

        private TResult InnerInvoke<T1, TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, Func<T1, TResult> func, T1 a1)
        {
            try
            {
                if (operationBehavior != null)
                {
                    return operationBehavior.Execute(context, func, a1);
                }
                else
                {
                    return func(a1);
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e, a1);
                throw new FaultException(e);
            }

        }

        #endregion

        #endregion

        #region PostInvoke

        #region PostInvoke Actions
        private void PostInvoke(ProxyActionBehavior<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null);
                });
            }
        }

        private void PostInvoke<T1>(ProxyActionBehavior<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null, a1);
                });
            }

        }
        #endregion

        #region PostInvoke Functions
        private void PostInvoke<TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context, TResult result)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, result);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, result);
                });
            }
        }

        private void PostInvoke<T1, TResult>(ProxyFunctionBehavior<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, TResult result, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, result, a1);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, result, a1);
                });
            }

        }
        #endregion

        #endregion

        #endregion

        #region Asynchronous

        #region Invoke Async

        #region Invoke Async Actions

        protected Task InvokeAsync(Func<Task> asyncAction, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(asyncAction, serviceMemberName);

            return ExecutePipelineAsync(
                () => AuthenticateInvoke(operationBehaviour, Context),
                () => AuthorizeInvoke(operationBehaviour, Context),
                () => PreInvoke(operationBehaviour, Context),
                () => operationBehaviour.Execute(Context, asyncAction),
                () => PostInvoke(operationBehaviour, Context)
                );
        }

        protected Task InvokeAsync<T1>(Func<T1, Task> asyncAction, T1 a1, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(asyncAction, serviceMemberName);

            return ExecutePipelineAsync(
                () => AuthenticateInvoke(operationBehaviour, Context, a1),
                () => AuthorizeInvoke(operationBehaviour, Context, a1),
                () => PreInvoke(operationBehaviour, Context, a1),
                () => operationBehaviour.Execute(Context, asyncAction, a1),
                () => PostInvoke(operationBehaviour, Context, a1)
            );
        }

        protected Task InvokeAsync<T1, T2>(Func<T1, T2, Task> asyncAction, T1 a1, T2 a2, [CallerMemberName] string serviceMemberName = null)
        {
            var operationBehaviour = ContractBehavior.Operation(asyncAction, serviceMemberName);

            return ExecutePipelineAsync(
                () => AuthenticateInvoke(operationBehaviour, Context, a1, a2),
                () => AuthorizeInvoke(operationBehaviour, Context, a1, a2),
                () => PreInvoke(operationBehaviour, Context, a1, a2),
                () => operationBehaviour.Execute(Context, asyncAction, a1, a2),
                () => PostInvoke(operationBehaviour, Context, a1, a2)
            );
        }

        protected Task InvokeAsync<T1, T2>(Func<T1, T2, Task> function, T1 arg1, T2 arg2)
        {
            return function(arg1, arg2);
        }

        #endregion

        #region Invoke Async Functions

        protected Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> funcAsync, [CallerMemberName] string memberName = null)
        {
            var operationBehavior = ContractBehavior.Operation(funcAsync, memberName);

            return ExecutePipelineWithResultAsync(
                () => AuthenticateInvoke(operationBehavior, Context),
                () => AuthorizeInvoke(operationBehavior, Context),
                () => PreInvoke(operationBehavior, Context),
                () => InnerInvokeAsync(operationBehavior, Context, funcAsync),
                (result) => PostInvoke(operationBehavior, Context, result)
            );
        }

        protected Task<TResult> InvokeAsync<T1, TResult>(Func<T1, Task<TResult>> funcAsync, T1 a1, [CallerMemberName] string memberName = null)
        {
            var operationBehavior = ContractBehavior.Operation(funcAsync, memberName);

            return ExecutePipelineWithResultAsync(
                () => AuthenticateInvoke(operationBehavior, Context, a1),
                () => AuthorizeInvoke(operationBehavior, Context, a1),
                () => PreInvoke(operationBehavior, Context, a1),
                () => InnerInvokeAsync(operationBehavior, Context, funcAsync, a1),
                (result) => PostInvoke(operationBehavior, Context, result, a1)
            );
        }

        #endregion

        #endregion

        #region Authenticate Async

        #region Authenticate Async Actions
        private void AuthenticateInvoke(ProxyActionBehaviorAsync<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthenticateInvoke<T1>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        private void AuthenticateInvoke<T1, T2>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> operationBehavior, TAmbientContext context, T1 a1, T2 a2)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1, a2);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
            }

        }

        #endregion

        #region Authenticate Async Functions
        private void AuthenticateInvoke<TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthenticateInvoke<T1, TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authenticate(context, a1);
            }
            else
            {
                ContractBehavior.AuthenticateHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }
        }

        #endregion

        #endregion

        #region Authorize Async

        #region Authorize Async Actions
        private void AuthorizeInvoke(ProxyActionBehaviorAsync<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthorizeInvoke<T1>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context, a1);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        private void AuthorizeInvoke<T1, T2>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> operationBehavior, TAmbientContext context, T1 a1, T2 a2)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context, a1, a2);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
            }

        }

        #endregion

        #region Authorize Async Functions

        private void AuthorizeInvoke<TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void AuthorizeInvoke<T1, TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.Authorize(context, a1);
            }
            else
            {
                ContractBehavior.AuthorizeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        #endregion

        #endregion

        #region PreInvoke Async

        #region PreInvoke Async Actions
        private void PreInvoke(ProxyActionBehaviorAsync<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void PreInvoke<T1>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        private void PreInvoke<T1, T2>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> operationBehavior, TAmbientContext context, T1 a1, T2 a2)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context, a1, a2);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1, a2);
                });
            }

        }
        #endregion

        #region PreInvoke Async Functions
        private void PreInvoke<TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context);
                });
            }
        }

        private void PreInvoke<T1, TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PreInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PreInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, a1);
                });
            }

        }

        #endregion

        #endregion

        #region PostInvoke Async

        #region PostInvoke Async Actions
        private void PostInvoke(ProxyActionBehaviorAsync<IContract, TAmbientContext> operationBehavior, TAmbientContext context)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null);
                });
            }
        }

        private void PostInvoke<T1>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1> operationBehavior, TAmbientContext context, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, a1);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null, a1);
                });
            }

        }

        private void PostInvoke<T1, T2>(ProxyActionBehaviorAsync<IContract, TAmbientContext, T1, T2> operationBehavior, TAmbientContext context, T1 a1, T2 a2)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, a1, a2);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null, a1, a2);
                });
            }

        }

        #endregion

        #region PostInvoke Async Functions
        private void PostInvoke<TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context, TResult result)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, result);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, null);
                });
            }
        }

        private void PostInvoke<T1, TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, TResult result, T1 a1)
        {
            if (operationBehavior != null)
            {
                operationBehavior.PostInvoke(context, result, a1);
            }
            else
            {
                ContractBehavior.PostInvokeHandlers.DoForEach((handler) =>
                {
                    handler(context, result, a1);
                });
            }

        }
        #endregion

        #endregion

        #region InnerInvoke Async

        #region InnverInvoke Async Actions

        private Task InnerInvokeAsync(TAmbientContext context, Func<Task> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvokeAsync", context, e);
                throw new FaultException(e);
            }

        }


        private Task InnerInvokeAsync<T1>(TAmbientContext context, Func<T1, Task> action, T1 a1)
        {
            try
            {
                return action(a1);
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvokeAsync", context, e, a1);
                throw new FaultException(e);
            }

        }

        private Task InnerInvokeAsync<T1, T2>(TAmbientContext context, Func<T1, T2, Task> action, T1 a1, T2 a2)
        {
            try
            {
                return action(a1, a2);
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvokeAsync", context, e, a1, a2);
                throw new FaultException(e);
            }

        }


        #endregion

        #region InnerInvoke Async Functions

        private Task<TResult> InnerInvokeAsync<TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, TResult> operationBehavior, TAmbientContext context, Func<Task<TResult>> func)
        {
            try
            {
                if (operationBehavior != null)
                {
                    return operationBehavior.Execute(context, func);
                }
                else
                {
                    return func();
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e);
                throw new FaultException(e);
            }

        }

        private Task<TResult> InnerInvokeAsync<T1, TResult>(ProxyFunctionBehaviorAsync<IContract, TAmbientContext, T1, TResult> operationBehavior, TAmbientContext context, Func<T1, Task<TResult>> func, T1 a1)
        {
            try
            {
                if (operationBehavior != null)
                {
                    return operationBehavior.Execute(context, func, a1);
                }
                else
                {
                    return func(a1);
                }
            }
            catch (Exception e)
            {
                ContractBehavior.RaiseError("InnerInvoke", context, e, a1);
                throw new FaultException(e);
            }

        }


        #endregion

        #endregion

        #endregion


    }


}
