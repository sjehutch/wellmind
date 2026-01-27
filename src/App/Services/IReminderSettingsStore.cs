namespace WellMind.Services;

public interface IReminderSettingsStore
{
    Task<bool> GetIsEnabledAsync(CancellationToken ct = default);
    Task<TimeSpan> GetTimeAsync(CancellationToken ct = default);
    Task SaveAsync(bool isEnabled, TimeSpan time, CancellationToken ct = default);
}
