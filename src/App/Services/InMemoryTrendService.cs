using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryTrendService : ITrendService
{
    private readonly ICheckInStore _checkInStore;

    public InMemoryTrendService(ICheckInStore checkInStore)
    {
        _checkInStore = checkInStore;
    }

    public async Task<IReadOnlyList<Trend>> GetWeeklyTrendsAsync(CancellationToken cancellationToken = default)
    {
        var checkIns = await _checkInStore.GetLastDaysAsync(7, cancellationToken);

        var energyValues = checkIns.Select(checkIn => checkIn.Energy).ToList();
        var stressValues = checkIns.Select(checkIn => checkIn.Stress).ToList();

        IReadOnlyList<Trend> trends =
        [
            BuildTrend("Energy", energyValues),
            BuildTrend("Stress", stressValues)
        ];

        return trends;
    }

    private static Trend BuildTrend(string label, IReadOnlyList<int> values)
    {
        var hasValues = values.Count > 0;
        var summary = hasValues
            ? $"Avg {values.Average():0.0}"
            : "No check-ins yet";

        return new Trend
        {
            Label = label,
            Values = values,
            Summary = summary,
            LastValue = hasValues ? values[^1] : null,
            Min = hasValues ? values.Min() : null,
            Max = hasValues ? values.Max() : null,
            Average = hasValues ? values.Average() : null
        };
    }
}
