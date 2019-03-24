using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Contracts;
using Unity.AutoRegistration;

namespace Tests.AutoRegistration
{

    [TestClass]
    public class AutoRegistrationFixture
    {
#if NET40TESTS
        private const string TESTCATEGORY = "NET40";
#else
        private const string TESTCATEGORY = "NETSTANDARD AND NET461";
#endif

        private Mock<IServiceCollection> _containerMock;
        private List<RegisterEvent> _registered;
        private IServiceCollection _container;
        private delegate void RegistrationCallback(ServiceDescriptor serviceDescriptor);
        private IServiceCollection _realContainer;

        [TestInitialize]
        public void SetUp()
        {
            _realContainer = new ServiceCollection();

            _containerMock = new Mock<IServiceCollection>();
            _registered = new List<RegisterEvent>();
            var setup = _containerMock
                .Setup(c => c.Add(It.IsAny<ServiceDescriptor>()));

            var callback = new RegistrationCallback((serviceDescriptor) =>
                {
                    _registered.Add(new RegisterEvent(serviceDescriptor));
                    _realContainer.Add(serviceDescriptor);
                });

            setup.Callback(callback);

            _container = _containerMock.Object;
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenContainerIsNull_ThrowsException()
        {
            _container = null;
            _container
                .ConfigureAutoRegistration();
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplingAutoRegistrationWithoutAnyRules_NothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplingAutoRegistrationWithOnlyAssemblyRules_NothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplyMethodIsNotCalled_AutoRegistrationDoesNotHappen()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register());

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenAssemblyIsExcluded_AutoRegistrationDoesNotHappenForItsTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register())
                .ExcludeAssemblies(If.ContainsType<TestCache>)
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenSystemAssembliesAreExcluded_AutoRegistrationDoesNotHappenForTheirTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<String>, Then.Register())
                .Include(If.Is<Uri>, Then.Register())
                .ExcludeSystemAssemblies()
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }


        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenTypeIsExcluded_AutoRegistrationDoesNotHappenForIt()
        {
            _container
                .ConfigureAutoRegistration()
                .Exclude(If.Is<TestCache>)
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenRegisterWithDefaultOptions_TypeMustBeRegisteredAsAllInterfacesItImplementsUsingTransientLifetime()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsTrue(_registered.Count == 2);

            var iCacheRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(ICache));
            var iDisposableRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(IDisposable));

            Assert.IsNotNull(iCacheRegisterEvent);
            Assert.IsNotNull(iDisposableRegisterEvent);
            Assert.AreEqual(typeof(TestCache), iCacheRegisterEvent.To);
            Assert.AreEqual(ServiceLifetime.Transient, iCacheRegisterEvent.Lifetime);
            Assert.AreEqual(typeof(TestCache), iDisposableRegisterEvent.To);
            Assert.AreEqual(ServiceLifetime.Transient, iDisposableRegisterEvent.Lifetime);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenRegistrationObjectIsPassed_RequestedTypeRegisteredAsExpected()
        {
            var registration = Then.Register();
            registration.Interfaces = new[] { typeof(ICache) };
            registration.ServiceLifetime = ServiceLifetime.Scoped;

            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, registration)
                .ApplyAutoRegistration();

            Assert.AreEqual(1, _registered.Count);
            var registerEvent = _registered.Single();
            Assert.AreEqual(typeof(TestCache), registerEvent.To);
            Assert.AreEqual(ServiceLifetime.Scoped, registerEvent.Lifetime);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenHaveMoreThanOneRegistrationRules_TypesRegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Implements<ICustomerRepository>,
                         Then.Register()
                             .AsSingleInterfaceOfType()
                             .UsingScopedMode())
                .Include(If.DecoratedWith<LoggerAttribute>, Then.Register().AsAllInterfacesOfType())
                .ApplyAutoRegistration();

            // 2 types implement ICustomerRepository, LoggerAttribute decorated type implement 2 interfaces
            Assert.AreEqual(4, _registered.Count);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenImplementsITypeNameMehtodCalled_ItWorksAsExpected()
        {
            Assert.IsTrue(typeof(CustomerRepository).ImplementsITypeName());
            Assert.IsTrue(typeof(Introduction).ImplementsITypeName());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenImplementsOpenGenericTypes_RegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(type => type.ImplementsOpenGeneric(typeof(IHandlerFor<>)),
                    Then.Register().AsFirstInterfaceOfType())
                .ApplyAutoRegistration();

            Assert.AreEqual(2, _registered.Count);
            Assert.IsTrue(_registered
                .Select(r => r.To)
                .SequenceEqual(new[] { typeof(DomainEventHandlerOne), typeof(DomainEventHandlerTwo) }));
            Assert.IsTrue(_registered
                .Select(r => r.From)
                .All(t => t == typeof(IHandlerFor<DomainEvent>)));

            Assert.AreEqual(2, _realContainer.BuildServiceProvider().GetServices<IHandlerFor<DomainEvent>>().Count());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenRegistrationOfOpenGenericType_RegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(type => If.ImplementsITypeName(type) && type.Equals(typeof(Filter<>)), Then.Register())
                .ApplyAutoRegistration();

            Assert.AreEqual(1, _registered.Count);
            Assert.IsTrue(_registered
                .Select(r => r.To)
                .SequenceEqual(new[] { typeof(Filter<>) }));
            Assert.IsTrue(_registered
                .Select(r => r.From)
                .All(t => t == typeof(IFilter<>)));

            var result = _realContainer.BuildServiceProvider().GetService<IFilter<string>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void TestPlatformUsesCorrectTargetPlatformAssembly()
        {
            System.Reflection.Assembly unityAutoRegistrationAssembly;
            unityAutoRegistrationAssembly = typeof(Unity.AutoRegistration.AutoRegistration).GetTypeInfo().Assembly;

            var targetFrameworkInformationAttribute = unityAutoRegistrationAssembly.GetCustomAttribute<TargetFrameworkInformationAttribute>();
            var runtimeTarget = unityAutoRegistrationAssembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();

            if (targetFrameworkInformationAttribute == null)
                throw new Exception(nameof(TargetFrameworkInformationAttribute) + " not found.");

            var targetFramework = targetFrameworkInformationAttribute.TargetFramework;

            if (targetFramework == TargetFramework.Unknown)
                throw new Exception(nameof(TargetFrameworkInformationAttribute) + " is not implemented for all platforms.");

            TargetFramework expectedTargetFramework;
#if NET461
            expectedTargetFramework = TargetFramework.net461;
#elif NETCOREAPP2_0
            expectedTargetFramework = TargetFramework.netstandard2_0;
#else
            throw new Exception("unknown testing platform");
#endif

            Assert.AreEqual(expectedTargetFramework, targetFramework);
        }

        private class RegisterEvent
        {
            public Type From { get; private set; }
            public Type To { get; private set; }
            public ServiceLifetime Lifetime { get; private set; }

            public RegisterEvent(ServiceDescriptor serviceDescriptor)
            {
                From = serviceDescriptor.ServiceType;
                To = serviceDescriptor.ImplementationType;
                Lifetime = serviceDescriptor.Lifetime;
            }
        }

        public class Introduction : IIntroduction
        {

        }

        public interface IIntroduction
        {
        }

        private void Example()
        {
            var container = new ServiceCollection();

            container
                .ConfigureAutoRegistration()
                .LoadAssemblyFrom("MyFancyPlugin.dll")
                .ExcludeSystemAssemblies()
                .ExcludeAssemblies(a => a.GetName().FullName.Contains("Test"))
                .Include(If.ImplementsSingleInterface, Then.Register().AsSingleInterfaceOfType().UsingSingletonMode())
                .Include(If.Implements<ILogger>, Then.Register().UsingTransientMode())
                .Include(If.ImplementsITypeName, Then.Register())
                .Include(If.Implements<ICustomerRepository>, Then.Register())
                .Include(If.Implements<IOrderRepository>,
                         Then.Register().AsSingleInterfaceOfType().UsingTransientMode())
                .Include(If.DecoratedWith<LoggerAttribute>,
                         Then.Register()
                             .As<IDisposable>())
                .Exclude(t => t.Name.Contains("Trace"))
                .ApplyAutoRegistration();
        }
    }
}