using Microsoft.Extensions.DependencyInjection;
using WellMind.Views;

namespace WellMind.Services;

public sealed class ShellNavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public ShellNavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public Task GoToCheckInAsync()
    {
        return Shell.Current.GoToAsync(nameof(CheckInPage));
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }

    public Task GoToResourceAsync(string title, string url)
    {
        var encodedTitle = Uri.EscapeDataString(title ?? string.Empty);
        var encodedUrl = Uri.EscapeDataString(url ?? string.Empty);
        return Shell.Current.GoToAsync($"{nameof(InAppBrowserPage)}?title={encodedTitle}&url={encodedUrl}");
    }

    public async Task OpenGentleReminderAsync()
    {
        var page = _services.GetRequiredService<GentleReminderPage>();
        await Shell.Current.Navigation.PushModalAsync(page);
    }

    public Task CloseModalAsync()
    {
        return Shell.Current.Navigation.PopModalAsync();
    }
}
