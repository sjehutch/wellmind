using System.Windows.Input;
using Microsoft.Maui.Graphics;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class HomeViewModel : BaseViewModel
{
    private readonly ITrendService _trendService;
    private readonly INavigationService _navigationService;
    private readonly ICheckInStore _checkInStore;
    private readonly ITipService _tipService;
    private readonly IResourceLinkService _resourceLinkService;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();
    private IReadOnlyList<Tip> _tips = Array.Empty<Tip>();
    private IReadOnlyList<ResourceLink> _links = Array.Empty<ResourceLink>();
    private bool _hasTips;
    private bool _hasLinks;
    private CheckIn? _todayCheckIn;
    private bool _hasTodayCheckIn;
    private bool _hasNoTodayCheckIn = true;
    private string _primaryActionText = "Start today's check-in";
    private string _todayEnergyValue = "—";
    private string _todayStressValue = "—";
    private string _todayFocusValue = "—";
    private string _todaySleepValue = "—";
    private bool _hasTodayRhythm;
    private double _todayRhythmValue = 0.2;
    private string _todayRhythmNote = "No check-in yet today.";
    private string _todayRhythmDescriptor = "Low";
    private string _todayRhythmPercent = "1.0 / 5";
    private Color _todayRhythmColor = Color.FromArgb("#D96C6C");

    public HomeViewModel(
        ITrendService trendService,
        INavigationService navigationService,
        ICheckInStore checkInStore,
        ITipService tipService,
        IResourceLinkService resourceLinkService)
    {
        _trendService = trendService;
        _navigationService = navigationService;
        _checkInStore = checkInStore;
        _tipService = tipService;
        _resourceLinkService = resourceLinkService;

        PrimaryActionCommand = new Command(async () => await _navigationService.GoToCheckInAsync());
        OpenLinkCommand = new Command<ResourceLink>(async link => await OpenLinkAsync(link));
        SummaryText = "A calm snapshot of how your week has been going.";

        _ = LoadAsync();
    }

    public string SummaryText { get; }

    public IReadOnlyList<Trend> Trends
    {
        get => _trends;
        private set => SetProperty(ref _trends, value);
    }

    public IReadOnlyList<Tip> Tips
    {
        get => _tips;
        private set => SetProperty(ref _tips, value);
    }

    public bool HasTips
    {
        get => _hasTips;
        private set => SetProperty(ref _hasTips, value);
    }

    public IReadOnlyList<ResourceLink> Links
    {
        get => _links;
        private set => SetProperty(ref _links, value);
    }

    public CheckIn? TodayCheckIn
    {
        get => _todayCheckIn;
        private set => SetProperty(ref _todayCheckIn, value);
    }

    public bool HasTodayCheckIn
    {
        get => _hasTodayCheckIn;
        private set => SetProperty(ref _hasTodayCheckIn, value);
    }

    public bool HasNoTodayCheckIn
    {
        get => _hasNoTodayCheckIn;
        private set => SetProperty(ref _hasNoTodayCheckIn, value);
    }

    public string PrimaryActionText
    {
        get => _primaryActionText;
        private set => SetProperty(ref _primaryActionText, value);
    }

    public string TodayEnergyValue
    {
        get => _todayEnergyValue;
        private set => SetProperty(ref _todayEnergyValue, value);
    }

    public string TodayStressValue
    {
        get => _todayStressValue;
        private set => SetProperty(ref _todayStressValue, value);
    }

    public string TodayFocusValue
    {
        get => _todayFocusValue;
        private set => SetProperty(ref _todayFocusValue, value);
    }

    public string TodaySleepValue
    {
        get => _todaySleepValue;
        private set => SetProperty(ref _todaySleepValue, value);
    }

    public double TodayRhythmValue
    {
        get => _todayRhythmValue;
        private set => SetProperty(ref _todayRhythmValue, value);
    }

    public string TodayRhythmNote
    {
        get => _todayRhythmNote;
        private set => SetProperty(ref _todayRhythmNote, value);
    }

    public string TodayRhythmDescriptor
    {
        get => _todayRhythmDescriptor;
        private set => SetProperty(ref _todayRhythmDescriptor, value);
    }

    public string TodayRhythmPercent
    {
        get => _todayRhythmPercent;
        private set => SetProperty(ref _todayRhythmPercent, value);
    }

    public Color TodayRhythmColor
    {
        get => _todayRhythmColor;
        private set => SetProperty(ref _todayRhythmColor, value);
    }

    public bool HasLinks
    {
        get => _hasLinks;
        private set => SetProperty(ref _hasLinks, value);
    }

    public bool HasTodayRhythm
    {
        get => _hasTodayRhythm;
        private set => SetProperty(ref _hasTodayRhythm, value);
    }

    public ICommand PrimaryActionCommand { get; }
    public ICommand OpenLinkCommand { get; }
    public async Task LoadAsync()
    {
        // Pull today's entry first so the primary action and summary reflect local-day state.
        TodayCheckIn = await _checkInStore.GetTodayAsync();
        HasTodayCheckIn = TodayCheckIn is not null;
        HasNoTodayCheckIn = !HasTodayCheckIn;
        PrimaryActionText = HasTodayCheckIn ? "Update today's check-in" : "Start today's check-in";
        UpdateTodayCheckInDisplay(TodayCheckIn);

        Trends = await _trendService.GetWeeklyTrendsAsync();
        Tips = await _tipService.GetGentleTipsAsync();
        Links = await _resourceLinkService.GetLinksAsync();
        UpdateTodayRhythm(TodayCheckIn);
        UpdateVisibility();
    }

    private Task OpenLinkAsync(ResourceLink? link)
    {
        if (link is null || string.IsNullOrWhiteSpace(link.Url))
        {
            return Task.CompletedTask;
        }

        var title = string.IsNullOrWhiteSpace(link.Title) ? "Learn more" : link.Title;
        return _navigationService.GoToResourceAsync(title, link.Url);
    }

    private void UpdateVisibility()
    {
        HasTips = Tips.Count > 0;
        HasLinks = HasTips && Links.Count > 0;
    }

    private void UpdateTodayRhythm(CheckIn? todayCheckIn)
    {
        if (todayCheckIn is null)
        {
            TodayRhythmValue = 0.2;
            TodayRhythmNote = "No check-in yet today.";
            TodayRhythmDescriptor = "Low";
            TodayRhythmPercent = "1.0 / 5";
            TodayRhythmColor = Color.FromArgb("#D96C6C");
            HasTodayRhythm = false;
            return;
        }

        TodayRhythmValue = CalculateRhythmValue(todayCheckIn);
        TodayRhythmNote = "Based on your latest check-in.";
        TodayRhythmDescriptor = DescribeRhythm(TodayRhythmValue);
        TodayRhythmPercent = $"{1 + (TodayRhythmValue * 4):0.0} / 5";
        TodayRhythmColor = MixColor("#D96C6C", "#5FBF7A", TodayRhythmValue);
        HasTodayRhythm = true;
    }

    private void UpdateTodayCheckInDisplay(CheckIn? todayCheckIn)
    {
        if (todayCheckIn is null)
        {
            TodayEnergyValue = "—";
            TodayStressValue = "—";
            TodayFocusValue = "—";
            TodaySleepValue = "—";
            return;
        }

        TodayEnergyValue = todayCheckIn.Energy.ToString();
        TodayStressValue = todayCheckIn.Stress.ToString();
        TodayFocusValue = todayCheckIn.Focus.ToString();
        TodaySleepValue = todayCheckIn.SleepQuality.ToString();
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
