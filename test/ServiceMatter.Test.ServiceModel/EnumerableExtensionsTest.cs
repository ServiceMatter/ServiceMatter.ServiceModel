using ServiceMatter.ServiceModel.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Service.Matter.Test.ServiceModel
{
    public class EnumerableExtensionsTest
    {

        private ITestOutputHelper _output { get; }

        public EnumerableExtensionsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoForEach()
        {

            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var list = new int[9];

            numbers.DoForEach(nr =>
            {
                list[nr - 1] = nr;
                nr++; // no effect on numbers array, not passed by ref
            });

            Assert.Equal(9, list.Length);

            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, numbers);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);

            var list2 = new int[9];

            numbers = null;
            numbers.DoForEach(nr =>
            {
                list2[nr - 1] = nr;
            });
            Assert.Equal(new int[9], list2);

        }

    }

}
