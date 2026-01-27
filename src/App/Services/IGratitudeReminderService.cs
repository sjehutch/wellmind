namespace WellMind.Services;

public interface IGratitudeReminderService
{
    Task ApplySettingsAsync(bool isEnabled, TimeSpan time, CancellationToken ct = default);
}
