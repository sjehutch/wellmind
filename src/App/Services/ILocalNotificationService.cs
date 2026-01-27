namespace WellMind.Services;

public interface ILocalNotificationService
{
    Task ScheduleDaily(string id, string title, string body, TimeSpan time, CancellationToken ct = default);
    Task Cancel(string id, CancellationToken ct = default);
}
