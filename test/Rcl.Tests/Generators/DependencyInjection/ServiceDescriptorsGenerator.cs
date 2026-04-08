using Microsoft.Extensions.DependencyInjection;
using Rcl.Ioc;

namespace Rcl.Tests.Generators.DependencyInjection;

internal static class ServiceDescriptorsGenerator
{
    public static IEnumerable<object[]> GetServiceDescriptors()
    {
        var testDataDirectory = Path.Combine(Path.GetTempPath(), "floss-app-test");
        var services = new ServiceCollection();
        services.RegisterServices(testDataDirectory);

        var testableServices = services
            .Where(d => !d.ServiceType.Namespace?.StartsWith("MudBlazor") ?? true)
            .Where(d => d.Lifetime != ServiceLifetime.Transient)
            .ToList();

        foreach (var descriptor in testableServices)
        {
            yield return new object[] { descriptor };
        }
    }
}