using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryTipService : ITipService
{
    private readonly ICheckInStore _checkInStore;

    public InMemoryTipService(ICheckInStore checkInStore)
    {
        _checkInStore = checkInStore;
    }

    public async Task<IReadOnlyList<Tip>> GetGentleTipsAsync(CancellationToken cancellationToken = default)
    {
        var checkIns = await _checkInStore.GetLastDaysAsync(7, cancellationToken);
        if (checkIns.Count == 0)
        {
            return Array.Empty<Tip>();
        }

        var tips = new List<Tip>();

        var stressAverage = checkIns.Average(checkIn => checkIn.Stress);
        if (stressAverage >= 4.0)
        {
            tips.Add(new Tip { Message = "Stress has been higher than usual. A two-minute reset might help." });
        }

        var sleepAverage = checkIns.Average(checkIn => checkIn.SleepQuality);
        if (sleepAverage <= 2.5)
        {
            tips.Add(new Tip { Message = "Sleep looks lower this week. A short wind-down could help tonight." });
        }

        var energyAverage = checkIns.Average(checkIn => checkIn.Energy);
        if (energyAverage <= 2.5)
        {
            tips.Add(new Tip { Message = "Energy looks a bit lower. A lighter task or brief pause could help." });
        }

        var focusAverage = checkIns.Average(checkIn => checkIn.Focus);
        if (focusAverage <= 2.5)
        {
            tips.Add(new Tip { Message = "Focus feels scattered. A five-minute single-task reset could help." });
        }

        if (tips.Count < 2)
        {
            tips.Add(new Tip { Message = "Try a micro-break: three slow breaths, stretch, then a sip of water." });
        }

        if (tips.Count < 3)
        {
            tips.Add(new Tip { Message = "Want to note anything that made today feel heavier?" });
        }

        if (tips.Count == 0)
        {
            tips.Add(new Tip { Message = "Steady week so far. Keep it gentle and sustainable." });
        }

        return tips.Take(3).ToList();
    }
}
