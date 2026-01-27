namespace WellMind.Services;

public sealed class GratitudeReminderService : IGratitudeReminderService
{
    private const string ReminderId = "wellmind.gratitude.daily";
    private static readonly string[] Bodies =
    {
        "Tiny pause: notice one small good thing.",
        "A small thank-you, if you feel like it.",
        "Just a moment—one good thing counts.",
        "Pause for a second. That's enough."
    };

    private readonly ILocalNotificationService _notifications;

    public GratitudeReminderService(ILocalNotificationService notifications)
    {
        _notifications = notifications;
    }

    public async Task ApplySettingsAsync(bool isEnabled, TimeSpan time, CancellationToken ct = default)
    {
        // If the user turns it off, we cancel and do nothing else.
        if (!isEnabled)
        {
            await _notifications.Cancel(ReminderId, ct);
            return;
        }

        // Pick one calm body line when scheduling.
        var body = Bodies[Random.Shared.Next(Bodies.Length)];
        await _notifications.ScheduleDaily(ReminderId, "A gentle reminder", body, time, ct);
    }
}
