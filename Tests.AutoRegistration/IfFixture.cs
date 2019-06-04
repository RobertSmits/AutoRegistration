using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection.AutoRegistration;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class IfFixture
    {
        private const string TESTCATEGORY = "NETSTANDARD AND NET461";

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void IfNotDecoratedWithAttribute_ShouldReturnFalse()
        {
            var result = typeof(TestLogger).DecoratedWith<IgnoreAttribute>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void IfDecoratedWithAttribute_ShouldReturnTrue()
        {
            var result = typeof(TestLogger).DecoratedWith<Contracts.LoggerAttribute>();
            Assert.IsTrue(result);
        }

    }
}
