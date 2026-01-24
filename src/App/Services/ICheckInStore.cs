using WellMind.Models;

namespace WellMind.Services;

public interface ICheckInStore
{
    Task<CheckIn?> GetTodayAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CheckIn>> GetLastDaysAsync(int days, CancellationToken ct = default);
    Task UpsertTodayAsync(CheckIn input, CancellationToken ct = default);
    Task DeleteAllAsync(CancellationToken ct = default);
}
