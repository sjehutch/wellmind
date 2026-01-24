using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class HomeViewModel : BaseViewModel
{
    private readonly ITrendService _trendService;
    private readonly INavigationService _navigationService;
    private readonly ICheckInService _checkInService;
    private readonly ITipService _tipService;
    private readonly IResourceLinkService _resourceLinkService;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();
    private IReadOnlyList<Tip> _tips = Array.Empty<Tip>();
    private IReadOnlyList<ResourceLink> _links = Array.Empty<ResourceLink>();
    private bool _hasTips;
    private bool _hasLinks;
    private bool _hasTodayRhythm;
    private double _todayRhythmValue = 0.2;
    private string _todayRhythmNote = "No check-in yet today.";
    private string _todayRhythmDescriptor = "Low";
    private string _todayRhythmPercent = "1.0 / 5";
    private Color _todayRhythmColor = Color.FromArgb("#D96C6C");

    public HomeViewModel(
        ITrendService trendService,
        INavigationService navigationService,
        ICheckInService checkInService,
        ITipService tipService,
        IResourceLinkService resourceLinkService)
    {
        _trendService = trendService;
        _navigationService = navigationService;
        _checkInService = checkInService;
        _tipService = tipService;
        _resourceLinkService = resourceLinkService;

        StartCheckInCommand = new Command(async () => await _navigationService.GoToCheckInAsync());
        OpenLinkCommand = new Command<ResourceLink>(async link => await OpenLinkAsync(link));
        SummaryText = "A calm snapshot of how your week has been going.";

        _checkInService.CheckInsChanged += HandleCheckInsChanged;
        _ = RefreshAsync();
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

    public ICommand StartCheckInCommand { get; }
    public ICommand OpenLinkCommand { get; }

    private void HandleCheckInsChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshAsync());
    }

    private async Task RefreshAsync()
    {
        Trends = await _trendService.GetWeeklyTrendsAsync();
        Tips = await _tipService.GetGentleTipsAsync();
        Links = await _resourceLinkService.GetLinksAsync();
        await UpdateTodayRhythmAsync();
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

    private async Task UpdateTodayRhythmAsync()
    {
        var recent = await _checkInService.GetRecentAsync(1);
        var today = DateTime.UtcNow.Date;
        var latestToday = recent.LastOrDefault(checkIn => checkIn.Date.Date == today);

        if (latestToday is null)
        {
            TodayRhythmValue = 0.2;
            TodayRhythmNote = "No check-in yet today.";
            TodayRhythmDescriptor = "Low";
            TodayRhythmPercent = "1.0 / 5";
            TodayRhythmColor = Color.FromArgb("#D96C6C");
            HasTodayRhythm = false;
            return;
        }

        TodayRhythmValue = CalculateRhythmValue(latestToday);
        TodayRhythmNote = "Based on your latest check-in.";
        TodayRhythmDescriptor = DescribeRhythm(TodayRhythmValue);
        TodayRhythmPercent = $"{1 + (TodayRhythmValue * 4):0.0} / 5";
        TodayRhythmColor = MixColor("#D96C6C", "#5FBF7A", TodayRhythmValue);
        HasTodayRhythm = true;
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
