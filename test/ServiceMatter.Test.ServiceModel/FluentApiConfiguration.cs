using Service.Matter.Test.ServiceModel.Scaffold.Contract;
using Service.Matter.Test.ServiceModel.Scaffold.Host;
using Service.Matter.Test.ServiceModel.Scaffold.Proxy;
using ServiceMatter.ServiceModel.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Service.Matter.Test.ServiceModel
{
    public class FluentApiConfigurationTest : IDisposable
    {
        private ITestOutputHelper _output { get; }

        // list of    methodname,interfacename,eventname,context,input,result
        private List<(string, string, string, object, object, object)> _callList = new List<(string, string, string, object, object, object)>();

        private Stopwatch _sw = new Stopwatch();
        private string _context;
        private ServiceFactory _sf;

        public FluentApiConfigurationTest(ITestOutputHelper output)
        {
            _output = output;
            _sw.Start();
            _context = Guid.NewGuid().ToString();

            //Startup code here
            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- proxy configuration started");

            _sf = new ServiceFactory(_context, ConfigureProxies<string>());

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- proxy configuration completed");

        }


        [Fact]
        public void Configuration_synchronous_handlers()
        {

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- start scenario ");


            var engineA = _sf.Create<IEngineA>();

            var inputA = new OperationARequestDto
            {
                In = Guid.NewGuid().ToString()
            };

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call A");

            var operationAResult = engineA.OperationAa(inputA);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call A");

            Assert.Equal(10, _callList.Count);

            AssertCall(_callList, 0, ("AllOperations", "IEngineA", "Authentication", _context, new object[] { inputA }, null));
            AssertCall(_callList, 1, ("OperationAa", "IEngineA", "Authentication", _context, inputA, null));
            AssertCall(_callList, 2, ("AllOperations", "IEngineA", "Authorization", _context, new object[] { inputA }, null));
            AssertCall(_callList, 3, ("OperationAa", "IEngineA", "Authorization", _context, inputA, null));
            AssertCall(_callList, 4, ("AllOperations", "IEngineA", "PreInvoke", _context, new object[] { inputA }, null));
            AssertCall(_callList, 5, ("OperationAa", "IEngineA", "PreInvoke", _context, inputA, null));
            AssertCall(_callList, 6, ("OperationAa", "IEngineA", "Interceptor Before", _context, inputA, null));
            AssertCall(_callList, 7, ("OperationAa", "IEngineA", "Interceptor After", _context, inputA, operationAResult));
            AssertCall(_callList, 8, ("AllOperations", "IEngineA", "PostInvoke", _context, new object[] { inputA }, operationAResult));
            AssertCall(_callList, 9, ("OperationAa", "IEngineA", "PostInvoke", _context, inputA, operationAResult));


            OperationBRequestDto inputB = new OperationBRequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call B");

            var operationBbResult = engineA.OperationBb(inputB);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call B");

            Assert.Equal(20, _callList.Count);

            AssertCall(_callList, 10, ("AllOperations", "IEngineA", "Authentication", _context, new object[] { inputB }, null));
            AssertCall(_callList, 11, ("OperationBb", "IEngineA", "Authentication", _context, inputB, null));
            AssertCall(_callList, 12, ("AllOperations", "IEngineA", "Authorization", _context, new object[] { inputB }, null));
            AssertCall(_callList, 13, ("OperationBb", "IEngineA", "Authorization", _context, inputB, null));
            AssertCall(_callList, 14, ("AllOperations", "IEngineA", "PreInvoke", _context, new object[] { inputB }, null));
            AssertCall(_callList, 15, ("OperationBb", "IEngineA", "PreInvoke", _context, inputB, null));
            AssertCall(_callList, 16, ("OperationBb", "IEngineA", "Interceptor Before", _context, inputB, null));
            AssertCall(_callList, 17, ("OperationBb", "IEngineA", "Interceptor After", _context, inputB, operationBbResult));
            AssertCall(_callList, 18, ("AllOperations", "IEngineA", "PostInvoke", _context, new object[] { inputB }, operationBbResult));
            AssertCall(_callList, 19, ("OperationBb", "IEngineA", "PostInvoke", _context, inputB, operationBbResult));


            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call C");

            OperationCRequestDto inputC = new OperationCRequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            var operationCcResult = engineA.OperationCc(inputC);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call C");

            Assert.Equal(24, _callList.Count);

            AssertCall(_callList, 20, ("AllOperations", "IEngineA", "Authentication", _context, new object[] { inputC }, null));
            AssertCall(_callList, 21, ("AllOperations", "IEngineA", "Authorization", _context, new object[] { inputC }, null));
            AssertCall(_callList, 22, ("AllOperations", "IEngineA", "PreInvoke", _context, new object[] { inputC }, null));
            AssertCall(_callList, 23, ("AllOperations", "IEngineA", "PostInvoke", _context, new object[] { inputC }, operationCcResult));

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call C");

            //Check whether minimal config works and does not produce null reference errors.
            var engineC = _sf.Create<IEngineC>();

            OperationARequestDto inputAC = new OperationARequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            var operationAcResult = engineC.OperationAa(inputAC);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call C");

            Assert.Equal(26, _callList.Count);

            AssertCall(_callList, 24, ("AllOperations", "IEngineC", "Authentication", _context, new object[] { inputAC }, null));
            AssertCall(_callList, 25, ("AllOperations", "IEngineC", "Authorization", _context, new object[] { inputAC }, null));


            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- completed ");
        }

        [Fact]
        public async Task Configuration_asynchronous_handlers()
        {
            _callList.Clear();

            var engineB = _sf.Create<IEngineB>();

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- before call Async");

            OperationARequestDto inputA = new OperationARequestDto
            {
                In = Guid.NewGuid().ToString(),
            };

            var operationAaAsyncResult = await engineB.OperationAaAsync(inputA);

            _output.WriteLine($"{_sw.ElapsedMilliseconds} ---- after call A Async");

//            Assert.Equal(10, _callList.Count);

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

            Func<OperationARequestDto, OperationAResultDto> func = A<IEngineA>.X.OperationAa;

            Assert.NotNull(func.Method);
            Assert.Equal(nameof(IEngineA.OperationAa), func.Method.Name);

        }

        [Fact]
        public void A_X_returns_a_ValueType()
        {

            var x = A<int>.X;

            Assert.Equal(default(int), x);

            var b = A<string>.X;

            Assert.Equal(default(string), b);

            var c = A<float>.X;

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
            where T: class
        {
            var _proxyConfig = new ProxyFactoryConfiguration<T>();

            _proxyConfig
                .For<IEngineA>()
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
                .For<IEngineB>()
                    .Use<EngineBProxy<T>>()
                    .AddAuthentication((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "Authentication", ctx, input, null)); })
                    .AddAuthorization((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "Authorization", ctx, input, null)); })
                    .AddPreInvoke((ctx, input) => { _callList.Add(("AllOperations", "IEngineB", "PreInvoke", ctx, input, null)); })
                    .AddPostInvoke((ctx, result, input) => { _callList.Add(("AllOperations", "IEngineB", "PostInvoke", ctx, input, result)); })
                    .ForOperation(Is.A<IEngineB>().OperationAaAsync, Is.A<OperationARequestDto>())
                        .AddAuthentication((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Authentication", c, a1, null)); })
                        .AddAuthorization((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Authorization", c, a1, null)); })
                        .AddPreInvoke((c, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "PreInvoke", c, a1, null)); })
                        .AddPostInvoke((c, r, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "PostInvoke", c, a1, r)); })
                        .AddInterceptor( async (c, f, a1) => { _callList.Add(("OperationAaAsync", "IEngineB", "Interceptor Before", c, a1, null)); var result = await f(a1); _callList.Add(("OperationAaAsync", "IEngineB", "Interceptor After", c, a1, result)); return result; })
            .ProxyConfiguration()
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
