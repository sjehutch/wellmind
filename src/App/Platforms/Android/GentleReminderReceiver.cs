using Android.Content;

namespace WellMind.Services;

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class GentleReminderReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        var id = intent.GetStringExtra(LocalNotificationScheduler.ExtraId) ?? "wellmind.gratitude.daily";
        var title = intent.GetStringExtra(LocalNotificationScheduler.ExtraTitle) ?? "A gentle reminder";
        var body = intent.GetStringExtra(LocalNotificationScheduler.ExtraBody) ?? "Tiny pause: notice one small good thing.";
        var ticks = intent.GetLongExtra(LocalNotificationScheduler.ExtraTimeTicks, new TimeSpan(9, 0, 0).Ticks);

        // Show the notification now.
        LocalNotificationScheduler.ShowNotification(context, id, title, body);

        // Schedule the next one for tomorrow at the same time.
        LocalNotificationScheduler.Schedule(context, id, title, body, TimeSpan.FromTicks(ticks));
    }
}
