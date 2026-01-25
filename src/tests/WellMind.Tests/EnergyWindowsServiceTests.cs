using WellMind.Models;
using WellMind.Services;
using Xunit;

namespace WellMind.Tests;

public sealed class EnergyWindowsServiceTests
{
    private readonly EnergyWindowsService _service = new();

    [Fact]
    public void ReturnsNoPatternWhenInsufficientData()
    {
        var message = _service.BuildMessage(new List<CheckIn>
        {
            Make("2026-01-01", 3, 3, 3, 3),
            Make("2026-01-02", 3, 3, 3, 3)
        });

        Assert.Equal("No clear pattern yet. That's normal.", message.Message);
        Assert.Equal("insufficient", message.ReasonKey);
    }

    [Fact]
    public void HighStressSteadySleepHasPriority()
    {
        var checkIns = BuildSeries(7, energy: 3, stress: 4, focus: 3, sleep: 3);

        var message = _service.BuildMessage(checkIns);

        Assert.Equal("high_stress_steady_sleep", message.ReasonKey);
    }

    [Fact]
    public void VariabilityMessageMatches()
    {
        var checkIns = new List<CheckIn>
        {
            Make("2026-01-01", 1, 3, 3, 3),
            Make("2026-01-02", 4, 3, 3, 3),
            Make("2026-01-03", 1, 3, 3, 3),
            Make("2026-01-04", 4, 3, 3, 3),
            Make("2026-01-05", 2, 3, 3, 3),
            Make("2026-01-06", 3, 3, 3, 3),
            Make("2026-01-07", 4, 3, 3, 3)
        };

        var message = _service.BuildMessage(checkIns);

        Assert.Equal("variability", message.ReasonKey);
    }

    [Fact]
    public void SteadyOkMessageMatches()
    {
        var checkIns = BuildSeries(7, energy: 3, stress: 2, focus: 3, sleep: 3);

        var message = _service.BuildMessage(checkIns);

        Assert.Equal("steady_ok", message.ReasonKey);
    }

    private static List<CheckIn> BuildSeries(int days, int energy, int stress, int focus, int sleep)
    {
        var list = new List<CheckIn>();
        for (var i = 0; i < days; i++)
        {
            var date = new DateTime(2026, 1, 1).AddDays(i).ToString("yyyy-MM-dd");
            list.Add(Make(date, energy, stress, focus, sleep));
        }

        return list;
    }

    private static CheckIn Make(string dateLocal, int energy, int stress, int focus, int sleep)
    {
        return new CheckIn
        {
            DateLocal = dateLocal,
            TimestampLocal = $"{dateLocal}T08:00:00",
            Energy = energy,
            Stress = stress,
            Focus = focus,
            SleepQuality = sleep
        };
    }
}
