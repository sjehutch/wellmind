using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.Maui.ApplicationModel;

namespace WellMind.Services;

public partial class LocalNotificationService
{
    private partial async Task ScheduleDailyPlatform(string id, string title, string body, TimeSpan time, CancellationToken ct)
    {
        // Ask for notification permission on Android 13+.
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            var status = await Permissions.RequestAsync<Permissions.Notifications>();
            if (status != PermissionStatus.Granted)
            {
                return;
            }
        }

        var context = Application.Context;
        // Schedule one alarm for the next time today (or tomorrow if time passed).
        LocalNotificationScheduler.Schedule(context, id, title, body, time);
    }

    private partial Task CancelPlatform(string id, CancellationToken ct)
    {
        var context = Application.Context;
        LocalNotificationScheduler.Cancel(context, id);
        return Task.CompletedTask;
    }
}

internal static class LocalNotificationScheduler
{
    internal const string ChannelId = "wellmind.gentle";
    internal const string ChannelName = "Gentle reminders";
    internal const string ExtraId = "id";
    internal const string ExtraTitle = "title";
    internal const string ExtraBody = "body";
    internal const string ExtraTimeTicks = "timeTicks";

    internal static void Schedule(Context context, string id, string title, string body, TimeSpan time)
    {
        EnsureChannel(context);

        var intent = new Intent(context, typeof(GentleReminderReceiver));
        intent.PutExtra(ExtraId, id);
        intent.PutExtra(ExtraTitle, title);
        intent.PutExtra(ExtraBody, body);
        intent.PutExtra(ExtraTimeTicks, time.Ticks);

        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            GetRequestCode(id),
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        // AlarmManager wakes us up at the right time so we can show a notification.
        var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        if (alarmManager is null)
        {
            return;
        }

        var triggerAtMillis = GetNextTriggerMillis(time);
        // Exact alarm keeps the reminder at the chosen time.
        alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
    }

    internal static void Cancel(Context context, string id)
    {
        var intent = new Intent(context, typeof(GentleReminderReceiver));
        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            GetRequestCode(id),
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        alarmManager?.Cancel(pendingIntent);
    }

    internal static void ShowNotification(Context context, string id, string title, string body)
    {
        EnsureChannel(context);

        // Use a stable id so future reminders replace the old one.
        var builder = new NotificationCompat.Builder(context, ChannelId)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetAutoCancel(true);

        NotificationManagerCompat.From(context).Notify(GetRequestCode(id), builder.Build());
    }

    private static void EnsureChannel(Context context)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return;
        }

        var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        if (manager is null)
        {
            return;
        }

        var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
        {
            Description = "Gentle daily reminders"
        };

        manager.CreateNotificationChannel(channel);
    }

    private static int GetRequestCode(string id)
    {
        return Math.Abs(id.GetHashCode());
    }

    private static long GetNextTriggerMillis(TimeSpan time)
    {
        var now = DateTime.Now;
        var target = now.Date + time;
        if (target <= now)
        {
            target = target.AddDays(1);
        }

        return new DateTimeOffset(target).ToUnixTimeMilliseconds();
    }
}
