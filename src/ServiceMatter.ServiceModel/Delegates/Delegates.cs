using System;
using System.Threading.Tasks;

namespace ServiceMatter.ServiceModel.Delegates
{
    public delegate void ContractPipelineEventHandler<TAmbientContext>(TAmbientContext context, params object[] input) where TAmbientContext : class;

    public delegate void PipelineEventHandler<TAmbientContext>(TAmbientContext context) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1>(TAmbientContext context, T1 a1) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1, T2>(TAmbientContext context, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1, T2, T3>(TAmbientContext context, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1, T2, T3, T4>(TAmbientContext context, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1, T2, T3, T4, T5>(TAmbientContext context, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate void PipelineEventHandler<TAmbientContext, T1, T2, T3, T4, T5, T6>(TAmbientContext context, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

    public delegate void PipelineResultEventHandler<TAmbientContext>(TAmbientContext context, object result, params object[] input) where TAmbientContext : class;

    public delegate void PipelineResultEventHandler<TAmbientContext, TResult>(TAmbientContext context, TResult result) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1>(TAmbientContext context, TResult result, T1 a1) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1, T2>(TAmbientContext context, TResult result, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1, T2, T3>(TAmbientContext context, TResult result, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1, T2, T3, T4>(TAmbientContext context, TResult result, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1, T2, T3, T4, T5>(TAmbientContext context, TResult result, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate void PipelineResultEventHandler<TAmbientContext, TResult, T1, T2, T3, T4, T5, T6>(TAmbientContext context, TResult result, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

    public delegate void ActionWrapper<TAmbientContext>(TAmbientContext context, Action action) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1>(TAmbientContext context, Action<T1> action, T1 a1) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1, T2>(TAmbientContext context, Action<T1, T2> action, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1, T2, T3>(TAmbientContext context, Action<T1, T2, T3> action, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1, T2, T3, T4>(TAmbientContext context, Action<T1, T2, T3, T4> action, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1, T2, T3, T4, T5>(TAmbientContext context, Action<T1, T2, T3, T4, T5> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate void ActionWrapper<TAmbientContext, T1, T2, T3, T4, T5, T6>(TAmbientContext context, Action<T1, T2, T3, T4, T5, T6> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

    public delegate TResult FunctionWrapper<TAmbientContext, TResult>(TAmbientContext context, Func<TResult> func) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1>(TAmbientContext context, Func<T1, TResult> func, T1 a1) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1, T2>(TAmbientContext context, Func<T1, T2, TResult> func, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1, T2, T3>(TAmbientContext context, Func<T1, T2, T3, TResult> func, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4>(TAmbientContext context, Func<T1, T2, T3, T4, TResult> func, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4, T5>(TAmbientContext context, Func<T1, T2, T3, T4, T5, TResult> func, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate TResult FunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4, T5, T6>(TAmbientContext context, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

    public delegate Task AsyncActionWrapper<TAmbientContext>(TAmbientContext context, Func<Task> action) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1>(TAmbientContext context, Func<T1,Task> action, T1 a1) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1, T2>(TAmbientContext context, Func<T1, T2, Task> action, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1, T2, T3>(TAmbientContext context, Func<T1, T2, T3, Task> action, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1, T2, T3, T4>(TAmbientContext context, Func<T1, T2, T3, T4, Task> action, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1, T2, T3, T4, T5>(TAmbientContext context, Func<T1, T2, T3, T4, T5, Task> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate Task AsyncActionWrapper<TAmbientContext, T1, T2, T3, T4, T5, T6>(TAmbientContext context, Func<T1, T2, T3, T4, T5, T6, Task> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult>(TAmbientContext context, Func<Task<TResult>> func) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1>(TAmbientContext context, Func<T1, Task<TResult>> func, T1 a1) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2>(TAmbientContext context, Func<T1, T2, Task<TResult>> func, T1 a1, T2 a2) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2, T3>(TAmbientContext context, Func<T1, T2, T3, Task<TResult>> func, T1 a1, T2 a2, T3 a3) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4>(TAmbientContext context, Func<T1, T2, T3, T4, Task<TResult>> func, T1 a1, T2 a2, T3 a3, T4 a4) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4, T5>(TAmbientContext context, Func<T1, T2, T3, T4, T5, Task<TResult>> func, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) where TAmbientContext : class;
    public delegate Task<TResult> AsyncFunctionWrapper<TAmbientContext, TResult, T1, T2, T3, T4, T5, T6>(TAmbientContext context, Func<T1, T2, T3, T4, T5, T6, Task<TResult>> func, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) where TAmbientContext : class;

}
