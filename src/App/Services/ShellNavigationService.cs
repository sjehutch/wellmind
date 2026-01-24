using WellMind.Views;

namespace WellMind.Services;

public sealed class ShellNavigationService : INavigationService
{
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
}
