using Microsoft.Extensions.DependencyInjection;

namespace WellMind.Services;

public sealed class ServiceRouteFactory : RouteFactory
{
    private readonly IServiceProvider _services;
    private readonly Type _pageType;

    public ServiceRouteFactory(IServiceProvider services, Type pageType)
    {
        _services = services;
        _pageType = pageType;
    }

    public override Element GetOrCreate()
    {
        return (Element)_services.GetRequiredService(_pageType);
    }
}
