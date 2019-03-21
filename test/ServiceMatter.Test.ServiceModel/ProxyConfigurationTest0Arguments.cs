using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Service.Matter.Test.ServiceModel.Scaffolding.Contract;
using Service.Matter.Test.ServiceModel.Scaffolding.Host;
using Service.Matter.Test.ServiceModel.Scaffolding.Proxy;
using ServiceMatter.ServiceModel.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Service.Matter.Test.ServiceModel
{
    public class ProxyConfigurationTest0Arguments : IDisposable
    {
        private ITestOutputHelper _output { get; }

        // list of    methodname,interfacename,eventname,context,input,result
        private List<(string, string, string, object, object, object)> _callList = new List<(string, string, string, object, object, object)>();

        private Stopwatch _sw = new Stopwatch();
        private readonly string _context;
        private ScaffoldingServiceFactory _sf;

        public ProxyConfigurationTest0Arguments(ITestOutputHelper output)
        {
            _output = output;
            _sw.Start();
            _context = Guid.NewGuid().ToString();

            //Startup code here
            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- proxy configuration started");

            _sf = new ScaffoldingServiceFactory(_context, ConfigureProxies<string>());

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- proxy configuration completed");

        }


        [Fact(Skip = "Scaffold for IVoidManager not implemented yet")]
        public void Synchronous_handlers_void()
        {
            //TODO VoidManager implementation
            var service = _sf.Create<IVoidManager>();

            service.NoArgs();

            AssertAllCalls(0, "NoArgs", "IVoidManager", null, null);

            var a1 = new ArgOne();

            service.OneArg(a1);

            AssertAllCalls(10, "OneArgs", "IVoidManager", null, a1);

            a1 = new ArgOne();
            var a2 = new ArgTwo();

            service.TwoArgs(a1, a2);

            AssertAllCalls(10, "OneArgs", "IVoidManager", null, a1, a2);

        }


        private void AssertAllCalls(int startIndex, string methodName, string interfaceName, object result, params object[] args)
        {
            Assert.Equal(startIndex + 10, _callList.Count);

            AssertCall(_callList, startIndex, ("AllOperations", interfaceName, "Authentication", _context, args, null));
            AssertCall(_callList, startIndex + 1, (methodName, interfaceName, "Authentication", _context, args, null));
            AssertCall(_callList, startIndex + 2, ("AllOperations", interfaceName, "Authorization", _context, args, null));
            AssertCall(_callList, startIndex + 3, (methodName, interfaceName, "Authorization", _context, args, null));
            AssertCall(_callList, startIndex + 4, ("AllOperations", interfaceName, "PreInvoke", _context, args, null));
            AssertCall(_callList, startIndex + 5, (methodName, interfaceName, "PreInvoke", _context, args, null));
            AssertCall(_callList, startIndex + 6, (methodName, interfaceName, "Interceptor Before", _context, args, null));
            AssertCall(_callList, startIndex + 7, (methodName, interfaceName, "Interceptor After", _context, args, result));
            AssertCall(_callList, startIndex + 8, ("AllOperations", interfaceName, "PostInvoke", _context, args, result));
            AssertCall(_callList, startIndex + 9, (methodName, interfaceName, "PostInvoke", _context, args, result));
        }

        [Fact]
        public async Task Configuration_asynchronous_handlers()
        {
            _callList.Clear();

            var engineB = _sf.Create<IEngineB>();


            var inputA = new OperationARequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call Async");

            var operationAaAsyncResult = await engineB.OperationAaAsync(inputA);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call A Async");

            Assert.Equal(10, _callList.Count);

            AssertCall(_callList, 0, ("AllOperations", "IEngineB", "Authentication", _context, new object[] { inputA }, null));
            AssertCall(_callList, 1, ("OperationAaAsync", "IEngineB", "Authentication", _context, inputA, null));
            AssertCall(_callList, 2, ("AllOperations", "IEngineB", "Authorization", _context, new object[] { inputA }, null));
            AssertCall(_callList, 3, ("OperationAaAsync", "IEngineB", "Authorization", _context, inputA, null));
            AssertCall(_callList, 4, ("AllOperations", "IEngineB", "PreInvoke", _context, new object[] { inputA }, null));
            AssertCall(_callList, 5, ("OperationAaAsync", "IEngineB", "PreInvoke", _context, inputA, null));
            AssertCall(_callList, 6, ("OperationAaAsync", "IEngineB", "Interceptor Before", _context, inputA, null));
            AssertCall(_callList, 7, ("OperationAaAsync", "IEngineB", "Interceptor After", _context, inputA, operationAaAsyncResult));
            AssertCall(_callList, 8, ("AllOperations", "IEngineB", "PostInvoke", _context, new object[] { inputA }, operationAaAsyncResult));
            AssertCall(_callList, 9, ("OperationAaAsync", "IEngineB", "PostInvoke", _context, inputA, operationAaAsyncResult));

            var inputB = new OperationBRequestDto
            {
                In = Guid.NewGuid().ToString(),
            };


            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call B");

            var operationBbAsyncResult = await engineB.OperationBbAsync(inputB);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call B");

            AssertCall(_callList, 10, ("AllOperations", "IEngineB", "Authentication", _context, new object[] { inputB }, null));
            AssertCall(_callList, 11, ("OperationBbAsync", "IEngineB", "Authentication", _context, inputB, null));
            AssertCall(_callList, 12, ("AllOperations", "IEngineB", "Authorization", _context, new object[] { inputB }, null));
            AssertCall(_callList, 13, ("OperationBbAsync", "IEngineB", "Authorization", _context, inputB, null));
            AssertCall(_callList, 14, ("AllOperations", "IEngineB", "PreInvoke", _context, new object[] { inputB }, null));
            AssertCall(_callList, 15, ("OperationBbAsync", "IEngineB", "PreInvoke", _context, inputB, null));
            AssertCall(_callList, 16, ("OperationBbAsync", "IEngineB", "Interceptor Before", _context, inputB, null));
            AssertCall(_callList, 17, ("OperationBbAsync", "IEngineB", "Interceptor After", _context, inputB, operationBbAsyncResult));
            AssertCall(_callList, 18, ("AllOperations", "IEngineB", "PostInvoke", _context, new object[] { inputB }, operationBbAsyncResult));
            AssertCall(_callList, 19, ("OperationBbAsync", "IEngineB", "PostInvoke", _context, inputB, operationBbAsyncResult));

            //Check whether minimal config works and does not produce null reference errors.
            var engineC = _sf.Create<IEngineC>();

            var inputBC = new OperationBRequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call engine C BbAsync");

            var result = await engineC.OperationBbAsync(inputBC);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call engine C BbAsync");

            Assert.Equal(22, _callList.Count);

            AssertCall(_callList, 20, ("AllOperations", "IEngineC", "Authentication", _context, new object[] { inputBC }, null));
            AssertCall(_callList, 21, ("AllOperations", "IEngineC", "Authorization", _context, new object[] { inputBC }, null));


            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- completed ");

        }

        [Fact]
        public void Stub_For_returns_a_DelegateMethod()
        {

            Func<OperationARequestDto, OperationAResultDto> func = Stub.For<IEngineA>().OperationAa;

            Assert.NotNull(func.Method);
            Assert.Equal(nameof(IEngineA.OperationAa), func.Method.Name);

        }

        [Fact]
        public void A_X_returns_a_DelegateMethod()
        {

            Func<OperationARequestDto, OperationAResultDto> func = An<IEngineA>.X.OperationAa;

            Assert.NotNull(func.Method);
            Assert.Equal(nameof(IEngineA.OperationAa), func.Method.Name);

        }

        [Fact]
        public void A_X_returns_a_ValueType()
        {

            var x = An<int>.X;

            Assert.Equal(default(int), x);

            var b = An<string>.X;

            Assert.Equal(default(string), b);

            var c = An<float>.X;

            Assert.Equal(default(float), c);

        }


        [Fact]
        public void Is_A_returns_a_DelegateMethod()
        {
            Func<OperationARequestDto, OperationAResultDto> func = Is.A<IEngineA>().OperationAa;

            Assert.NotNull(func.Method);
            Assert.Equal(nameof(IEngineA.OperationAa), func.Method.Name);

        }


        private ProxyFactoryConfiguration<T> ConfigureProxies<T>()
            where T : class
        {
            var _proxyConfig = new ProxyFactoryConfiguration<T>();

            _proxyConfig
                .For<IEngineA>()
                    // EngineB - SYNCHRONOUS full configuration
                    .Use<EngineAProxy<T>>()
                    .AddAuthentication((ctx, input) => { _callList.Add(("AllOperations", "IEngineA", "Authentication", ctx, input, null)); })
                    .AddAuthorization((ctx, input) => { _callList.Add(("AllOperations", "IEngineA", "Authorization", ctx, input, null)); })
                    .AddPreInvoke((ctx, input) => { _callList.Add(("AllOperations", "IEngineA", "PreInvoke", ctx, input, null)); })
                    .AddPostInvoke((ctx, result, input) => { _callList.Add(("AllOperations", "IEngineA", "PostInvoke", ctx, input, result)); })
                    .ForOperation(Is.A<IEngineA>().OperationAa, Is.A<OperationARequestDto>())
                        .AddAuthentication((c, a1) => { _callList.Add(("OperationAa", "IEngineA", "Authentication", c, a1, null)); })
                        .AddAuthorization((c, a1) => { _callList.Add(("OperationAa", "IEngineA", "Authorization", c, a1, null)); })
                        .AddPreInvoke((c, a1) => { _callList.Add(("OperationAa", "IEngineA", "PreInvoke", c, a1, null)); })
                        .AddPostInvoke((c, r, a1) => { _callList.Add(("OperationAa", "IEngineA", "PostInvoke", c, a1, r)); })
                        .AddInterceptor((c, f, a1) => { _callList.Add(("OperationAa", "IEngineA", "Interceptor Before", c, a1, null)); var result = f(a1); _callList.Add(("OperationAa", "IEngineA", "Interceptor After", c, a1, result)); return result; })
                     .Contract()
                     .ForOperation(Stub.For<IEngineA>().OperationBb, Is.A<OperationBRequestDto>())
                        .AddAuthentication((c, a1) => { _callList.Add(("OperationBb", "IEngineA", "Authentication", c, a1, null)); })
                        .AddAuthorization((c, a1) => { _callList.Add(("OperationBb", "IEngineA", "Authorization", c, a1, null)); })
                        .AddPreInvoke((c, a1) => { _callList.Add(("OperationBb", "IEngineA", "PreInvoke", c, a1, null)); })
                        .AddPostInvoke((c, r, a1) => { _callList.Add(("OperationBb", "IEngineA", "PostInvoke", c, a1, r)); })
                        .AddInterceptor((c, f, a1) => { _callList.Add(("OperationBb", "IEngineA", "Interceptor Before", c, a1, null)); var result = f(a1); _callList.Add(("OperationBb", "IEngineA", "Interceptor After", c, a1, result)); return result; })
            .ProxyConfiguration()
                // EngineB - ASYNCHRONOUS full configuration
                .For<IEngineB>()
                    .Use<EngineBProxy<T>>()
                    .AddAuthentication((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "Authentication", ctx, input, null)); })
                    .AddAuthorization((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "Authorization", ctx, input, null)); })
                    .AddPreInvoke((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "PreInvoke", ctx, input, null)); })
                    .AddPostInvoke((ctx, result, input) => { _callList.Add(("AllOperations", "IEngineB", "PostInvoke", ctx, input, result)); })
                    .ForOperation(An<IEngineB>.o.OperationAaAsync, An<OperationARequestDto>.Arg)
                        .AddAuthentication((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Authentication", c, a1, null)); })
                        .AddAuthorization((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Authorization", c, a1, null)); })
                        .AddPreInvoke((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "PreInvoke", c, a1, null)); })
                        .AddPostInvoke((c, r, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "PostInvoke", c, a1, r)); })
                        .AddInterceptor(async (c, f, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Interceptor Before", c, a1, null)); var result = await f(a1); _callList.Add(("OperationAaAsync", "IEngineB", "Interceptor After", c, a1, result)); return result; })
                    .Contract()
                    .ForOperation(An<IEngineB>.o.OperationBbAsync, An<OperationBRequestDto>.Arg)
                        //                    .ForAsyncMethod<OperationBRequestDto,OperationBResultDto>(x => x.OperationBbAsync(An<OperationBRequestDto>.Arg))
                        .AddAuthentication((c, a1) => { _callList.Add(("OperationBbAsync", "IEngineB", "Authentication", c, a1, null)); })
                        .AddAuthorization((c, a1) => { _callList.Add(("OperationBbAsync", "IEngineB", "Authorization", c, a1, null)); })
                        .AddPreInvoke((c, a1) => { _callList.Add(("OperationBbAsync", "IEngineB", "PreInvoke", c, a1, null)); })
                        .AddPostInvoke((c, r, a1) => { _callList.Add(("OperationBbAsync", "IEngineB", "PostInvoke", c, a1, r)); })
                        .AddInterceptor(async (c, f, a1) => { _callList.Add(("OperationBbAsync", "IEngineB", "Interceptor Before", c, a1, null)); var result = await f(a1); _callList.Add(("OperationBbAsync", "IEngineB", "Interceptor After", c, a1, result)); return result; })
            .ProxyConfiguration()
            // EngineC - minimal configuration SYNC AND ASYNC
                .For<IEngineC>()
                    .Use<EngineCProxy<T>>()
                    .AddAuthentication((ctx, input) => { _callList.Add(("AllOperations", "IEngineC", "Authentication", ctx, input, null)); })
                    .AddAuthorization((ctx, input) => { _callList.Add(("AllOperations", "IEngineC", "Authorization", ctx, input, null)); })
                    ;

            return _proxyConfig;
        }

        private void AssertCall(List<(string, string, string, object, object, object)> callList, int v, (string, string, string, object, object, object) p)
        {
            var item = callList[v];

            Assert.Equal(p.Item1, item.Item1);
            Assert.Equal(p.Item2, item.Item2);
            Assert.Equal(p.Item3, item.Item3);
            Assert.Equal(p.Item4, item.Item4);
            Assert.Equal(p.Item5, item.Item5);
            Assert.Equal(p.Item6, item.Item6);
        }


        public void Dispose()
        {
        }
    }
}
