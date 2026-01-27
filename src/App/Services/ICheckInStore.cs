using WellMind.Models;

namespace WellMind.Services;

public interface ICheckInStore
{
    Task<CheckIn?> GetTodayAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CheckIn>> GetLastDaysAsync(int days, CancellationToken ct = default);
    Task UpsertTodayAsync(CheckIn input, CancellationToken ct = default);
    Task DeleteAllAsync(CancellationToken ct = default);
    Task<string?> GetTodayHeavyNoteAsync(CancellationToken ct = default);
    Task<DateTime?> GetTodayHeavyNoteUpdatedAtAsync(CancellationToken ct = default);
    Task UpsertTodayHeavyNoteAsync(string? note, DateTime updatedAt, CancellationToken ct = default);
}
