using Microsoft.Extensions.DependencyInjection;
using Rcl.Ioc;
using Rcl.Tests.Generators.DependencyInjection;

namespace Rcl.Tests.Tests.Ioc;

[TestClass]
public class RclServiceRegistrarTests
{
    [TestMethod]
    [DynamicData(
        nameof(ServiceDescriptorsGenerator.GetServiceDescriptors),
        typeof(ServiceDescriptorsGenerator))]
    public void RegisterServices_Can_Manufacture_Each_Registered_Service(ServiceDescriptor descriptor)
    {
        var testDataDirectory = Path.Combine(Path.GetTempPath(), "floss-app-test");
        var services = new ServiceCollection();
        services.RegisterServices(testDataDirectory);

        using var provider = services.BuildServiceProvider();
        var resolved = provider.GetService(descriptor.ServiceType);

        Assert.IsNotNull(resolved);
    }
}