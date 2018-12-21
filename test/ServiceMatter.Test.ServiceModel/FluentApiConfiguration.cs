using System;
using Service.Matter.Test.ServiceModel.Scaffold.Contract;
using Service.Matter.Test.ServiceModel.Scaffold.Proxy;
using ServiceMatter.ServiceModel.Configuration;
using Xunit;

namespace Service.Matter.Test.ServiceModel
{
    public class FluentApiConfigurationTest
    {
        [Fact]
        public void Startup_configuration_all_handlers() {

            //Must configure the proxy pipeline at app startup
            ServiceModelConfiguration<string>
            .ProxyConfiguration
                .For<IEngineA>()
                    .Use<EngineAProxy<string>>()
                    .AddAuthentication((ctx, input) => { })
                    .AddAuthorization((ctx, input) => { })
                    .AddPreInvoke((ctx, input) => { })
                    .AddPostInvoke((ctx, result, input) => { })
                    .ForOperation(Is.A<IEngineA>().OperationAa, Is.A<OperationARequestDto>())
                        .AddAuthentication((c, a1) => { })
                        .AddAuthorization((c, a1) => { })
                        .AddPreInvoke((c, a1) => { })
                        .AddPostInvoke((c, r, a1) => { })
                        .AddInterceptor((c, f, a1) => f(a1))
                     .Contract()
                     .ForOperation(Stub.For<IEngineA>().OperationBb, Is.A<OperationBRequestDto>())
                        .AddAuthentication((ctx, input) => { })
                        .AddAuthorization((ctx, input) => { })
                        .AddPreInvoke((ctx, input) => { })
                        .AddPostInvoke((ctx, result, input) => { })
                        .AddInterceptor((c, f, a1) => { return f(a1); });


        }

        [Fact]
        public void Stub_For_returns_a_DelegateMethod() {

            Func<OperationARequestDto, OperationAResultDto> func = Stub.For<IEngineA>().OperationAa;

            Assert.NotNull(func.Method);
            Assert.Equal(nameof(IEngineA.OperationAa),func.Method.Name);

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
    }
}
