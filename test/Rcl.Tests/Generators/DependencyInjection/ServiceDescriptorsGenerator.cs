using Microsoft.Extensions.DependencyInjection;
using Rcl.Ioc;

namespace Rcl.Tests.Generators.DependencyInjection;

internal static class ServiceDescriptorsGenerator
{
    public static IEnumerable<object[]> GetServiceDescriptors()
    {
        var services = new ServiceCollection();
        services.RegisterServices();

        foreach (var descriptor in services)
        {
            yield return new object[] { descriptor };
        }
    }
}