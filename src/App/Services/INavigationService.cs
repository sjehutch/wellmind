namespace WellMind.Services;

public interface INavigationService
{
    Task GoToCheckInAsync();
    Task GoBackAsync();
    Task GoToResourceAsync(string title, string url);
    Task OpenGentleReminderAsync();
    Task OpenHistoryReminderAsync();
    Task CloseModalAsync();
}
