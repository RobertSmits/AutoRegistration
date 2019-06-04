using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection.AutoRegistration;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class ExtensionFixture
    {
        private const string TESTCATEGORY = "NETSTANDARD AND NET461";

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void GetAttributeWithInvalidAttribute_ShouldThrowException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(ExtensionFixture).GetAttribute<TestMethodAttribute>();
            });
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void GetAttributeWithValidAttribute_ShouldReturnAttribute()
        {
            var result = typeof(ExtensionFixture).GetAttribute<TestClassAttribute>();
            Assert.IsNotNull(result);
        }
    }
}
