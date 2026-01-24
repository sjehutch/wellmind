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
}
