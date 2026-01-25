using Microsoft.Maui.Graphics;
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
            .Select(checkIn =>
            {
                var rhythmValue = CalculateRhythmValue(checkIn);
                return new PastCheckInItem(
                    checkIn.DateLocal,
                    ToScoreText(rhythmValue),
                    DescribeRhythm(rhythmValue),
                    ToScoreValue(rhythmValue),
                    MixColor("#D96C6C", "#5FBF7A", rhythmValue));
            })
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

    private static double ToScoreValue(double value)
    {
        return Math.Clamp(1 + (value * 4), 1, 5);
    }

    private static Color MixColor(string startHex, string endHex, double t)
    {
        t = Math.Clamp(t, 0, 1);
        var start = Color.FromArgb(startHex);
        var end = Color.FromArgb(endHex);

        return new Color(
            (float)(start.Red + (end.Red - start.Red) * t),
            (float)(start.Green + (end.Green - start.Green) * t),
            (float)(start.Blue + (end.Blue - start.Blue) * t));
    }
}

public sealed record PastCheckInItem(string DateLocal, string Score, string Descriptor, double ScoreValue, Color ScoreColor);
