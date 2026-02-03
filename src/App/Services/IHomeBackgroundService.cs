namespace WellMind.Services;

public interface IHomeBackgroundService
{
    // Gets the saved background color hex, or null if nothing is set.
    Task<string?> GetBackgroundColorAsync();
    // Saves the chosen color as the app background.
    Task SetBackgroundColorAsync(string colorHex);
    // Clears the saved background and goes back to default.
    Task ResetAsync();
    // True when a background is enabled in settings.
    bool IsEnabled { get; }
}
