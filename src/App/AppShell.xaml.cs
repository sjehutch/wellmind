using Microsoft.Extensions.DependencyInjection;
using WellMind.Features.Support.TalkToSomeone;
using WellMind.Pages;
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

        Items.Add(new ShellContent
        {
            Title = "Past check-ins",
            Content = services.GetRequiredService<PastCheckInsPage>()
        });

        Items.Add(new ShellContent
        {
            Title = "How we score",
            Content = services.GetRequiredService<ScoreExplanationPage>()
        });

        Items.Add(new ShellContent
        {
            Title = "Privacy, by design",
            Content = services.GetRequiredService<PrivacyCommitmentPage>()
        });

        Items.Add(new ShellContent
        {
            Title = "Grounded",
            Content = services.GetRequiredService<GroundedPage>()
        });

        Items.Add(new ShellContent
        {
            Title = "Reading",
            Content = services.GetRequiredService<ReadingListPage>()
        });

        var supportFlyout = new FlyoutItem { Title = "Support" };
        supportFlyout.Items.Add(new ShellContent
        {
            Title = "Talk to Someone",
            Content = services.GetRequiredService<TalkToSomeonePage>()
        });
        Items.Add(supportFlyout);

        // Route uses DI so pages can take ViewModels in their constructors.
        Routing.RegisterRoute(nameof(CheckInPage), new ServiceRouteFactory(services, typeof(CheckInPage)));
        Routing.RegisterRoute(nameof(InAppBrowserPage), new ServiceRouteFactory(services, typeof(InAppBrowserPage)));
    }
}
