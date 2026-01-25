using System.Linq;
using WellMind.Models;

namespace WellMind.Services;

public sealed class EnergyWindowsService : IEnergyWindowsService
{
    public EnergyWindowsResult BuildMessage(IReadOnlyList<CheckIn> checkIns)
    {
        if (checkIns.Count < 3)
        {
            return new EnergyWindowsResult("No clear pattern yet. That's normal.", "insufficient");
        }

        var energyAvg = checkIns.Average(checkIn => checkIn.Energy);
        var stressAvg = checkIns.Average(checkIn => checkIn.Stress);
        var focusAvg = checkIns.Average(checkIn => checkIn.Focus);
        var sleepAvg = checkIns.Average(checkIn => checkIn.SleepQuality);

        var energyRange = Range(checkIns.Select(checkIn => checkIn.Energy));
        var stressRange = Range(checkIns.Select(checkIn => checkIn.Stress));
        var focusRange = Range(checkIns.Select(checkIn => checkIn.Focus));
        var sleepRange = Range(checkIns.Select(checkIn => checkIn.SleepQuality));

        if (stressAvg >= 4 && sleepAvg >= 3)
        {
            return new EnergyWindowsResult(
                "Stress has been running high, even with decent sleep. That happens.",
                "high_stress_steady_sleep");
        }

        if (energyAvg <= 2 && focusAvg <= 2)
        {
            return new EnergyWindowsResult(
                "Energy and focus have been lower this week. Many people experience this.",
                "low_energy_low_focus");
        }

        if (energyRange >= 3 || stressRange >= 3 || focusRange >= 3 || sleepRange >= 3)
        {
            return new EnergyWindowsResult(
                "Your week has had some ups and downs. This can vary week to week.",
                "variability");
        }

        if (energyAvg >= 3 && stressAvg <= 3)
        {
            return new EnergyWindowsResult(
                "Things look fairly steady this week. That happens.",
                "steady_ok");
        }

        return new EnergyWindowsResult(
            "You're getting a read on your week. This can vary week to week.",
            "fallback");
    }

    private static int Range(IEnumerable<int> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
        {
            return 0;
        }

        return list.Max() - list.Min();
    }
}
