using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace WebAPI.UnitTests;

public static class WebHostTestingExtensions
{
    public static WebApplicationFactory<TEntryPoint> Arrange<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, Action<WebFactoryArrangeOptions<TEntryPoint>> action)
    where TEntryPoint : class
    {
        var options = new WebFactoryArrangeOptions<TEntryPoint>(factory);
        action(options);
        return options.GetFactory();
    }
}

public class WebFactoryArrangeOptions<TEntryPoint>
where TEntryPoint : class
{
    private WebApplicationFactory<TEntryPoint> factory;
    internal WebApplicationFactory<TEntryPoint> GetFactory() => factory;
    internal WebFactoryArrangeOptions(WebApplicationFactory<TEntryPoint> factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Requires an interface to be registered and Mocks it. 
    /// The interface is required to be already registered, otherwise this method will throw.        
    /// </summary>
    public Mock<TInterface> MockRequired<TInterface>()
    where TInterface : class
    {
        Mock<TInterface> mock = new();

        factory = factory
            .WithWebHostBuilder(app => app
                .ConfigureServices(services =>
                {
                    var pubEP = services.FirstOrDefault(s => s.ServiceType == typeof(TInterface));
                    if (pubEP != null)
                        services.Remove(pubEP);
                        
                    services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(TInterface),
                    _ => mock.Object,
                                Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient));
                }));

        return mock;   
    }
}
