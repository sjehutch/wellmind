using WellMind.Models;

namespace WellMind.Services;

public interface ICheckInService
{
    Task SaveAsync(CheckIn checkIn, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CheckIn>> GetRecentAsync(int days, CancellationToken cancellationToken = default);
    event EventHandler? CheckInsChanged;
}
