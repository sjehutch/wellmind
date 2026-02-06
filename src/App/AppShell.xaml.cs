using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using WellMind.Features.Support.TalkToSomeone;
using WellMind.Pages;
using WellMind.Services;
using WellMind.Views;

namespace WellMind;

public partial class AppShell : Shell
{
    private ShellNavigationState? _lastTabState;

    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        var tabs = new TabBar();

        tabs.Items.Add(new ShellContent
        {
            Title = "Home",
            Icon = CreateTabIcon("home"),
            Content = services.GetRequiredService<HomePage>()
        });

        tabs.Items.Add(new ShellContent
        {
            Title = "Check-in",
            Icon = CreateTabIcon("check_box"),
            Content = services.GetRequiredService<CheckInPage>()
        });

        tabs.Items.Add(new ShellContent
        {
            Title = "Reading",
            Icon = CreateTabIcon("menu_book"),
            Content = services.GetRequiredService<ReadingListPage>()
        });

        tabs.Items.Add(new ShellContent
        {
            Title = "Past",
            Icon = CreateTabIcon("history"),
            Content = services.GetRequiredService<PastCheckInsPage>()
        });

        tabs.Items.Add(new ShellContent
        {
            Title = "More",
            Icon = CreateTabIcon("more_horiz"),
            Content = services.GetRequiredService<MorePage>()
        });

        Items.Add(tabs);
        Navigated += OnShellNavigated;

        // Secondary pages navigated from More.
        Routing.RegisterRoute(nameof(PrivacyCommitmentPage), new ServiceRouteFactory(services, typeof(PrivacyCommitmentPage)));
        Routing.RegisterRoute(nameof(ScoreExplanationPage), new ServiceRouteFactory(services, typeof(ScoreExplanationPage)));
        Routing.RegisterRoute(nameof(GroundedPage), new ServiceRouteFactory(services, typeof(GroundedPage)));
        Routing.RegisterRoute(nameof(TalkToSomeonePage), new ServiceRouteFactory(services, typeof(TalkToSomeonePage)));

        // Route uses DI so pages can take ViewModels in their constructors.
        Routing.RegisterRoute(nameof(CheckInPage), new ServiceRouteFactory(services, typeof(CheckInPage)));
        Routing.RegisterRoute(nameof(InAppBrowserPage), new ServiceRouteFactory(services, typeof(InAppBrowserPage)));
        Routing.RegisterRoute(nameof(ReadingListPage), new ServiceRouteFactory(services, typeof(ReadingListPage)));
    }

    private static FontImageSource CreateTabIcon(string glyph)
    {
        var size = DeviceInfo.Platform == DevicePlatform.iOS ? 24 : 22;

        return new FontImageSource
        {
            FontFamily = "MaterialIcons",
            Glyph = glyph,
            Size = size
        };
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
#if IOS
        if (e.Source is ShellNavigationSource.ShellItemChanged or ShellNavigationSource.ShellSectionChanged)
        {
            if (_lastTabState is null || _lastTabState.Location != e.Current.Location)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }
#endif

        _lastTabState = e.Current;
    }
}
