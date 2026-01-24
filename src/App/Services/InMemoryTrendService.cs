using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryTrendService : ITrendService
{
    public Task<IReadOnlyList<Trend>> GetWeeklyTrendsAsync(CancellationToken cancellationToken = default)
    {
        // Placeholder trend data for the home screen tiles.
        IReadOnlyList<Trend> trends =
        [
            new Trend { Label = "Energy", Values = [3, 3, 4, 2, 3, 4, 3] },
            new Trend { Label = "Stress", Values = [2, 3, 2, 3, 3, 2, 2] }
        ];

        return Task.FromResult(trends);
    }
}
