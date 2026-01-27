using Microsoft.Maui.Storage;

namespace WellMind.Services;

public sealed class ReminderSettingsStore : IReminderSettingsStore
{
    // Keys for tiny, local settings.
    private const string EnabledKey = "gentle.reminder.enabled";
    private const string TimeKey = "gentle.reminder.time";
    private static readonly TimeSpan DefaultTime = new(9, 0, 0);

    public Task<bool> GetIsEnabledAsync(CancellationToken ct = default)
    {
        // A simple local setting: false means no reminder.
        return Task.FromResult(Preferences.Get(EnabledKey, false));
    }

    public Task<TimeSpan> GetTimeAsync(CancellationToken ct = default)
    {
        // Store the time as "HH:mm" so it is easy to read and parse.
        var raw = Preferences.Get(TimeKey, DefaultTime.ToString("hh\\:mm"));
        if (TimeSpan.TryParseExact(raw, "hh\\:mm", null, out var parsed))
        {
            return Task.FromResult(parsed);
        }

        return Task.FromResult(DefaultTime);
    }

    public Task SaveAsync(bool isEnabled, TimeSpan time, CancellationToken ct = default)
    {
        // Persist both values so we can restore the toggle and time later.
        Preferences.Set(EnabledKey, isEnabled);
        Preferences.Set(TimeKey, time.ToString("hh\\:mm"));
        return Task.CompletedTask;
    }
}
