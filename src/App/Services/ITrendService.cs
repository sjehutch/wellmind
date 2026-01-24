using WellMind.Models;

namespace WellMind.Services;

public interface ITrendService
{
    Task<IReadOnlyList<Trend>> GetWeeklyTrendsAsync(CancellationToken cancellationToken = default);
}
