using Microsoft.Extensions.DependencyInjection;
using WellMind.Services;
using WellMind.Views;

namespace WellMind;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        Items.Add(new ShellContent
        {
            Title = "Home",
            Content = services.GetRequiredService<HomePage>()
        });

        // Route uses DI so pages can take ViewModels in their constructors.
        Routing.RegisterRoute(nameof(CheckInPage), new ServiceRouteFactory(services, typeof(CheckInPage)));
        Routing.RegisterRoute(nameof(InAppBrowserPage), new ServiceRouteFactory(services, typeof(InAppBrowserPage)));
    }
}
