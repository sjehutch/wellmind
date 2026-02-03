using Microsoft.Maui.Storage;

namespace WellMind.Services;

public sealed class HomeBackgroundService : IHomeBackgroundService
{
    private const string EnabledKey = "HomeBackgroundEnabled";
    private const string ColorKey = "HomeBackgroundColor";

    public bool IsEnabled => Preferences.Default.Get(EnabledKey, false);

    public Task<string?> GetBackgroundColorAsync()
    {
        if (!IsEnabled)
        {
            return Task.FromResult<string?>(null);
        }

        var color = Preferences.Default.Get(ColorKey, string.Empty);
        if (string.IsNullOrWhiteSpace(color))
        {
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(color);
    }

    public Task SetBackgroundColorAsync(string colorHex)
    {
        Preferences.Default.Set(ColorKey, colorHex);
        Preferences.Default.Set(EnabledKey, true);
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        Preferences.Default.Remove(ColorKey);
        Preferences.Default.Remove(EnabledKey);
        return Task.CompletedTask;
    }
}
