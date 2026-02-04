using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryTipService : ITipService
{
    private readonly ICheckInStore _checkInStore;
    private readonly ITonePackStore _tonePackStore;
    private readonly ITipFeedbackStore _tipFeedbackStore;

    public InMemoryTipService(
        ICheckInStore checkInStore,
        ITonePackStore tonePackStore,
        ITipFeedbackStore tipFeedbackStore)
    {
        _checkInStore = checkInStore;
        _tonePackStore = tonePackStore;
        _tipFeedbackStore = tipFeedbackStore;
    }

    public async Task<IReadOnlyList<Tip>> GetGentleTipsAsync(CancellationToken cancellationToken = default)
    {
        var tonePack = await _tonePackStore.GetAsync(cancellationToken);
        var checkIns = await _checkInStore.GetLastDaysAsync(7, cancellationToken);
        if (checkIns.Count == 0)
        {
            return Array.Empty<Tip>();
        }

        var tips = new List<Tip>();

        var stressAverage = checkIns.Average(checkIn => checkIn.Stress);
        if (stressAverage >= 4.0)
        {
            tips.Add(new Tip
            {
                Id = "stress.high",
                Message = "Stress has been higher than usual. A two-minute reset might help."
            });
        }

        var sleepAverage = checkIns.Average(checkIn => checkIn.SleepQuality);
        if (sleepAverage <= 2.5)
        {
            tips.Add(new Tip
            {
                Id = "sleep.low",
                Message = "Sleep looks lower this week. A short wind-down could help tonight."
            });
        }

        var energyAverage = checkIns.Average(checkIn => checkIn.Energy);
        if (energyAverage <= 2.5)
        {
            tips.Add(new Tip
            {
                Id = "energy.low",
                Message = "Energy looks a bit lower. A lighter task or brief pause could help."
            });
        }

        var focusAverage = checkIns.Average(checkIn => checkIn.Focus);
        if (focusAverage <= 2.5)
        {
            tips.Add(new Tip
            {
                Id = "focus.low",
                Message = "Focus feels scattered. A five-minute single-task reset could help."
            });
        }

        if (tips.Count < 2)
        {
            tips.Add(new Tip
            {
                Id = $"reset.{tonePack.ToStorageValue()}",
                Message = GetResetTip(tonePack)
            });
        }

        if (tips.Count < 3)
        {
            tips.Add(new Tip
            {
                Id = $"reflection.{tonePack.ToStorageValue()}",
                Message = GetReflectionPrompt(tonePack)
            });
        }

        if (tips.Count == 0)
        {
            tips.Add(new Tip
            {
                Id = $"steady.{tonePack.ToStorageValue()}",
                Message = GetSteadyMessage(tonePack)
            });
        }

        var ranked = new List<(Tip tip, int score)>(tips.Count);
        foreach (var tip in tips)
        {
            var score = await _tipFeedbackStore.GetHelpfulCountAsync(tip.Id, cancellationToken);
            ranked.Add((tip, score));
        }

        return ranked
            .OrderByDescending(item => item.score)
            .Select(item => item.tip)
            .Take(3)
            .ToList();
    }

    private static string GetResetTip(TonePack tonePack)
    {
        return tonePack switch
        {
            TonePack.Focus => "Try a focus reset: one priority, ten minutes, notifications off.",
            TonePack.Recovery => "Try a recovery reset: unclench your jaw, drop your shoulders, sip water.",
            TonePack.Confidence => "Try a confidence reset: name one thing you handled well today.",
            _ => "Try a grounding reset: three slow breaths, then feel both feet on the floor."
        };
    }

    private static string GetReflectionPrompt(TonePack tonePack)
    {
        return tonePack switch
        {
            TonePack.Focus => "What's one task that would make today feel clearer?",
            TonePack.Recovery => "What would make the next hour feel 5% lighter?",
            TonePack.Confidence => "What evidence says you're handling more than you think?",
            _ => "What's one small thing helping you feel steadier today?"
        };
    }

    private static string GetSteadyMessage(TonePack tonePack)
    {
        return tonePack switch
        {
            TonePack.Focus => "Steady week so far. Keep your next step simple and clear.",
            TonePack.Recovery => "Steady week so far. Keep your pace kind and sustainable.",
            TonePack.Confidence => "Steady week so far. Keep trusting the progress you're building.",
            _ => "Steady week so far. Keep it gentle and sustainable."
        };
    }
}
