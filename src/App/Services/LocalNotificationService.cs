namespace WellMind.Services;

public partial class LocalNotificationService : ILocalNotificationService
{
    public Task ScheduleDaily(string id, string title, string body, TimeSpan time, CancellationToken ct = default)
    {
        // Each platform knows how to schedule its own local notification.
        return ScheduleDailyPlatform(id, title, body, time, ct);
    }

    public Task Cancel(string id, CancellationToken ct = default)
    {
        // Cancel on each platform in the simplest possible way.
        return CancelPlatform(id, ct);
    }

    // Platform-specific implementations live in Platforms/Android and Platforms/iOS.
    private partial Task ScheduleDailyPlatform(string id, string title, string body, TimeSpan time, CancellationToken ct);
    private partial Task CancelPlatform(string id, CancellationToken ct);
}
