using Foundation;
using UserNotifications;

namespace WellMind.Services;

public partial class LocalNotificationService
{
    private partial async Task ScheduleDailyPlatform(string id, string title, string body, TimeSpan time, CancellationToken ct)
    {
        var center = UNUserNotificationCenter.Current;

        // Ask permission the first time we schedule a reminder.
        var auth = await center.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound);
        if (!auth.Item1)
        {
            return;
        }

        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = body,
            Sound = UNNotificationSound.Default
        };

        var dateComponents = new NSDateComponents
        {
            Hour = time.Hours,
            Minute = time.Minutes
        };

        var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, true);
        var request = UNNotificationRequest.FromIdentifier(id, content, trigger);

        await center.AddNotificationRequestAsync(request);
    }

    private partial Task CancelPlatform(string id, CancellationToken ct)
    {
        var center = UNUserNotificationCenter.Current;
        center.RemovePendingNotificationRequests(new[] { id });
        return Task.CompletedTask;
    }
}
