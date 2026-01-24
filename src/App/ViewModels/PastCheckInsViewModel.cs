using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class PastCheckInsViewModel : BaseViewModel
{
    private readonly ICheckInStore _checkInStore;
    private IReadOnlyList<PastCheckInItem> _items = Array.Empty<PastCheckInItem>();

    public PastCheckInsViewModel(ICheckInStore checkInStore)
    {
        _checkInStore = checkInStore;
    }

    public IReadOnlyList<PastCheckInItem> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    public async Task LoadAsync()
    {
        var checkIns = await _checkInStore.GetLastDaysAsync(30);

        Items = checkIns
            .OrderByDescending(checkIn => checkIn.DateLocal)
            .Select(checkIn => new PastCheckInItem(
                checkIn.DateLocal,
                ToScoreText(CalculateRhythmValue(checkIn)),
                DescribeRhythm(CalculateRhythmValue(checkIn))))
            .ToList();
    }

    private static double CalculateRhythmValue(CheckIn checkIn)
    {
        var energy = Normalize(checkIn.Energy);
        var focus = Normalize(checkIn.Focus);
        var sleep = Normalize(checkIn.SleepQuality);
        var stress = 1 - Normalize(checkIn.Stress);

        return Math.Clamp((energy + focus + sleep + stress) / 4, 0, 1);
    }

    private static double Normalize(int value)
    {
        return Math.Clamp((value - 1) / 4.0, 0, 1);
    }

    private static string DescribeRhythm(double value)
    {
        if (value < 0.34)
        {
            return "Low";
        }

        if (value < 0.67)
        {
            return "Steady";
        }

        return "Strong";
    }

    private static string ToScoreText(double value)
    {
        return $"{1 + (value * 4):0.0} / 5";
    }
}

public sealed record PastCheckInItem(string DateLocal, string Score, string Descriptor);
