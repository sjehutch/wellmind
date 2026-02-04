using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Devices;
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
    private readonly IEnergyWindowsService _energyWindowsService;
    private readonly IReminderSettingsStore _reminderSettingsStore;
    private readonly IHomeBackgroundService _homeBackgroundService;
    private readonly ITonePackStore _tonePackStore;
    private readonly ITipFeedbackStore _tipFeedbackStore;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();
    private IReadOnlyList<Tip> _tips = Array.Empty<Tip>();
    private IReadOnlyList<ResourceLink> _links = Array.Empty<ResourceLink>();
    private bool _hasTips;
    private bool _hasLinks;
    private CheckIn? _todayCheckIn;
    private bool _hasTodayCheckIn;
    private bool _hasNoTodayCheckIn = true;
    private string _primaryActionText = "Start today's check-in";
    private string _greetingText = "Good morning";
    private string _todayDateText = $"Today • {DateTime.Now.ToString("dddd, MMMM d", CultureInfo.CurrentCulture)}";
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
    private string _energyWindowsMessage = "No pattern yet. That's normal.";
    private bool _showEnergyWindows;
    private bool _isGentleReminderEnabled;
    private TimeSpan _gentleReminderTime = new(9, 0, 0);
    private string _gentleReminderStatusText = "Off";
    private string _gentleReminderButtonText = "Set reminder";
    private bool _isHeavyNoteExpanded;
    private string _heavyNoteText = "";
    private DateTime? _heavyNoteUpdatedAt;
    private CancellationTokenSource? _heavyNoteSaveCts;
    private bool _isLoadingHeavyNote;
    private IDispatcherTimer? _greetingTimer;
    private Color _backgroundColor = Colors.Transparent;
    private bool _showBackgroundColor;
    private TonePack _selectedTonePack = TonePack.Grounding;
    private string _tonePackStatusText = "Current tone: Grounding";
    private string _tipFeedbackStatusText = string.Empty;

    public HomeViewModel(
        ITrendService trendService,
        INavigationService navigationService,
        ICheckInStore checkInStore,
        ITipService tipService,
        IResourceLinkService resourceLinkService,
        IEnergyWindowsService energyWindowsService,
        IReminderSettingsStore reminderSettingsStore,
        IHomeBackgroundService homeBackgroundService,
        ITonePackStore tonePackStore,
        ITipFeedbackStore tipFeedbackStore)
    {
        _trendService = trendService;
        _navigationService = navigationService;
        _checkInStore = checkInStore;
        _tipService = tipService;
        _resourceLinkService = resourceLinkService;
        _energyWindowsService = energyWindowsService;
        _reminderSettingsStore = reminderSettingsStore;
        _homeBackgroundService = homeBackgroundService;
        _tonePackStore = tonePackStore;
        _tipFeedbackStore = tipFeedbackStore;

        PrimaryActionCommand = new Command(async () => await StartCheckInAsync());
        OpenLinkCommand = new Command<ResourceLink>(async link => await OpenLinkAsync(link));
        ShowEnergyWindowsInfoCommand = new Command(async () => await ShowEnergyWindowsInfoAsync());
        OpenGentleReminderCommand = new Command(async () => await OpenGentleReminderAsync());
        OpenHistoryReminderCommand = new Command(async () => await OpenHistoryReminderAsync());
        SetTonePackCommand = new Command<string>(async tone => await SetTonePackAsync(tone));
        MarkTipHelpfulCommand = new Command<Tip>(async tip => await MarkTipHelpfulAsync(tip));
        ToggleHeavyNoteExpandedCommand = new Command(() => IsHeavyNoteExpanded = true);
        DoneHeavyNoteCommand = new Command(async () => await SaveHeavyNoteAndCollapseAsync());
        ClearHeavyNoteCommand = new Command(async () => await ClearHeavyNoteAsync());
        OpenBackgroundMenuCommand = new Command(async () => await OpenBackgroundMenuAsync());
        SummaryText = "A calm snapshot of how your week has been going.";

        EnsureGreetingTimerStarted();
        _ = LoadAsync();
    }

    public string SummaryText { get; }

    public string GreetingText
    {
        get => _greetingText;
        private set => SetProperty(ref _greetingText, value);
    }

    public string TodayDateText
    {
        get => _todayDateText;
        private set => SetProperty(ref _todayDateText, value);
    }

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

    public string EnergyWindowsMessage
    {
        get => _energyWindowsMessage;
        private set => SetProperty(ref _energyWindowsMessage, value);
    }

    public bool ShowEnergyWindows
    {
        get => _showEnergyWindows;
        private set => SetProperty(ref _showEnergyWindows, value);
    }

    public bool IsGentleReminderEnabled
    {
        get => _isGentleReminderEnabled;
        private set => SetProperty(ref _isGentleReminderEnabled, value);
    }

    public TimeSpan GentleReminderTime
    {
        get => _gentleReminderTime;
        private set => SetProperty(ref _gentleReminderTime, value);
    }

    public string GentleReminderStatusText
    {
        get => _gentleReminderStatusText;
        private set => SetProperty(ref _gentleReminderStatusText, value);
    }

    public string GentleReminderButtonText
    {
        get => _gentleReminderButtonText;
        private set => SetProperty(ref _gentleReminderButtonText, value);
    }

    public bool IsHeavyNoteExpanded
    {
        get => _isHeavyNoteExpanded;
        set => SetProperty(ref _isHeavyNoteExpanded, value);
    }

    public string HeavyNoteText
    {
        get => _heavyNoteText;
        set
        {
            if (string.Equals(_heavyNoteText, value, StringComparison.Ordinal))
            {
                return;
            }

            SetProperty(ref _heavyNoteText, value);

            // Ignore edits while we are loading saved data.
            if (_isLoadingHeavyNote)
            {
                return;
            }

            // Start a quiet, delayed save after typing pauses.
            DebounceHeavyNoteSave();
            NotifyHeavyNoteDerived();
        }
    }

    public string HeavyNotePreview
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_heavyNoteText))
            {
                return "Want to note anything that made today feel heavier?";
            }

            var trimmed = _heavyNoteText.Trim();
            return trimmed.Length <= 80 ? trimmed : $"{trimmed[..80]}…";
        }
    }

    public string HeavyNoteSavedLabel
    {
        get
        {
            if (_heavyNoteUpdatedAt is null || string.IsNullOrWhiteSpace(_heavyNoteText))
            {
                return string.Empty;
            }

            return $"Saved {_heavyNoteUpdatedAt.Value.ToString("h:mm tt")}";
        }
    }

    public bool HasHeavyNote
    {
        get => !string.IsNullOrWhiteSpace(_heavyNoteText);
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

    public Color BackgroundColor
    {
        get => _backgroundColor;
        private set => SetProperty(ref _backgroundColor, value);
    }

    public bool ShowBackgroundColor
    {
        get => _showBackgroundColor;
        private set => SetProperty(ref _showBackgroundColor, value);
    }

    public TonePack SelectedTonePack
    {
        get => _selectedTonePack;
        private set => SetProperty(ref _selectedTonePack, value);
    }

    public string TonePackStatusText
    {
        get => _tonePackStatusText;
        private set => SetProperty(ref _tonePackStatusText, value);
    }

    public string TipFeedbackStatusText
    {
        get => _tipFeedbackStatusText;
        private set
        {
            SetProperty(ref _tipFeedbackStatusText, value);
            RaisePropertyChanged(nameof(HasTipFeedbackStatus));
        }
    }

    public bool HasTipFeedbackStatus => !string.IsNullOrWhiteSpace(TipFeedbackStatusText);

    public bool IsGroundingToneSelected => SelectedTonePack == TonePack.Grounding;
    public bool IsFocusToneSelected => SelectedTonePack == TonePack.Focus;
    public bool IsRecoveryToneSelected => SelectedTonePack == TonePack.Recovery;
    public bool IsConfidenceToneSelected => SelectedTonePack == TonePack.Confidence;

    public ICommand PrimaryActionCommand { get; }
    public ICommand OpenLinkCommand { get; }
    public ICommand ShowEnergyWindowsInfoCommand { get; }
    public ICommand OpenGentleReminderCommand { get; }
    public ICommand OpenHistoryReminderCommand { get; }
    public ICommand SetTonePackCommand { get; }
    public ICommand MarkTipHelpfulCommand { get; }
    public ICommand ToggleHeavyNoteExpandedCommand { get; }
    public ICommand DoneHeavyNoteCommand { get; }
    public ICommand ClearHeavyNoteCommand { get; }
    public ICommand OpenBackgroundMenuCommand { get; }
    public async Task LoadAsync()
    {
        // Keep the greeting and date in sync with the user's current local time.
        UpdateGreetingAndDate();

        // Pull today's entry first so the primary action and summary reflect local-day state.
        TodayCheckIn = await _checkInStore.GetTodayAsync();
        HasTodayCheckIn = TodayCheckIn is not null;
        HasNoTodayCheckIn = !HasTodayCheckIn;
        PrimaryActionText = HasTodayCheckIn ? "Update today's check-in" : "Start today's check-in";
        UpdateTodayCheckInDisplay(TodayCheckIn);

        await LoadHeavyNoteAsync();
        await LoadGentleReminderStatusAsync();
        await LoadTonePackAsync();
        await RefreshBackgroundAsync();

        Trends = await _trendService.GetWeeklyTrendsAsync();
        Tips = await _tipService.GetGentleTipsAsync();
        Links = await _resourceLinkService.GetLinksAsync();
        try
        {
            var recent = await _checkInStore.GetLastDaysAsync(7);
            EnergyWindowsMessage = _energyWindowsService.BuildMessage(recent).Message;
            ShowEnergyWindows = true;
        }
        catch
        {
            // If data is missing or unavailable, hide the card entirely.
            ShowEnergyWindows = false;
        }
        UpdateTodayRhythm(TodayCheckIn);
        UpdateVisibility();
    }

    private async Task StartCheckInAsync()
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        await _navigationService.GoToCheckInAsync();
    }

    private Task OpenGentleReminderAsync()
    {
        // Open the gentle reminder modal (no navigation stack changes).
        return _navigationService.OpenGentleReminderAsync();
    }

    private Task OpenHistoryReminderAsync()
    {
        return _navigationService.OpenHistoryReminderAsync();
    }

    private async Task SetTonePackAsync(string? tone)
    {
        if (!TonePackExtensions.TryParse(tone, out var parsedTonePack))
        {
            return;
        }

        if (SelectedTonePack == parsedTonePack)
        {
            return;
        }

        SelectedTonePack = parsedTonePack;
        TonePackStatusText = $"Current tone: {SelectedTonePack.ToDisplayName()}";
        RaiseToneSelectionChanged();
        await _tonePackStore.SaveAsync(parsedTonePack);
        Tips = await _tipService.GetGentleTipsAsync();
        UpdateVisibility();
    }

    private async Task MarkTipHelpfulAsync(Tip? tip)
    {
        if (tip is null || string.IsNullOrWhiteSpace(tip.Id))
        {
            return;
        }

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Some devices do not support haptics.
        }

        await _tipFeedbackStore.MarkHelpfulAsync(tip.Id);
        TipFeedbackStatusText = "Thanks. We'll prioritize tips like this.";
        Tips = await _tipService.GetGentleTipsAsync();
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
        HasLinks = Links.Count > 0;
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

    private static async Task ShowEnergyWindowsInfoAsync()
    {
        await Shell.Current.DisplayAlert(
            "About Energy Windows",
            "Energy Windows is a calm summary of your last 7 days of check-ins.\nIt looks for simple patterns, like steadiness or ups and downs.\nIt's not a diagnosis, and it's not instructions.\nNo action is required.",
            "OK");
    }

    private async Task OpenBackgroundMenuAsync()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Some devices do not support haptics.
        }

        var page = Application.Current?.MainPage;
        if (page is null)
        {
            return;
        }

        var action = await page.DisplayActionSheet(
            "Make this space yours",
            "Not now",
            null,
            "Soft Sage",
            "Warm Sand",
            "Sky Mist",
            "Lavender Haze",
            "Blush Rose",
            "Ocean Calm",
            "Stone Gray",
            "Midnight Teal",
            "Back to default");

        var colorHex = action switch
        {
            "Soft Sage" => "#DDE7E1",
            "Warm Sand" => "#EFE3D3",
            "Sky Mist" => "#DCE7F2",
            "Lavender Haze" => "#E7DDF0",
            "Blush Rose" => "#F2D6DE",
            "Ocean Calm" => "#D6E9E7",
            "Stone Gray" => "#E5E5E5",
            "Midnight Teal" => "#0F3D3E",
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(colorHex))
        {
            await _homeBackgroundService.SetBackgroundColorAsync(colorHex);
            await RefreshBackgroundAsync();
            return;
        }

        if (action == "Back to default")
        {
            await _homeBackgroundService.ResetAsync();
            await RefreshBackgroundAsync();
        }
    }

    private async Task RefreshBackgroundAsync()
    {
        var colorHex = await _homeBackgroundService.GetBackgroundColorAsync();
        if (string.IsNullOrWhiteSpace(colorHex))
        {
            BackgroundColor = Colors.Transparent;
            ShowBackgroundColor = false;
            return;
        }

        if (!Color.TryParse(colorHex, out var parsed))
        {
            BackgroundColor = Colors.Transparent;
            ShowBackgroundColor = false;
            return;
        }

        BackgroundColor = parsed;
        ShowBackgroundColor = true;
    }

    private async Task LoadGentleReminderStatusAsync()
    {
        // Pull the saved reminder settings so the UI can show status later.
        IsGentleReminderEnabled = await _reminderSettingsStore.GetIsEnabledAsync();
        GentleReminderTime = await _reminderSettingsStore.GetTimeAsync();
        GentleReminderStatusText = IsGentleReminderEnabled
            ? $"Daily at {DateTime.Today.Add(GentleReminderTime).ToString("h:mm tt")}"
            : "Off";
        GentleReminderButtonText = IsGentleReminderEnabled ? "Adjust reminder" : "Set reminder";
    }

    private async Task LoadTonePackAsync()
    {
        SelectedTonePack = await _tonePackStore.GetAsync();
        TonePackStatusText = $"Current tone: {SelectedTonePack.ToDisplayName()}";
        RaiseToneSelectionChanged();
    }

    private void RaiseToneSelectionChanged()
    {
        RaisePropertyChanged(nameof(IsGroundingToneSelected));
        RaisePropertyChanged(nameof(IsFocusToneSelected));
        RaisePropertyChanged(nameof(IsRecoveryToneSelected));
        RaisePropertyChanged(nameof(IsConfidenceToneSelected));
    }

    private void UpdateGreetingAndDate()
    {
        // Simple time-based greeting: morning before 12, afternoon 12–5, evening after 5.
        var hour = DateTime.Now.Hour;
        if (hour < 12)
        {
            GreetingText = "Good morning";
        }
        else if (hour < 17)
        {
            GreetingText = "Good afternoon";
        }
        else
        {
            GreetingText = "Good evening";
        }

        TodayDateText = $"Today • {DateTime.Now.ToString("dddd, MMMM d", CultureInfo.CurrentCulture)}";
    }

    private void EnsureGreetingTimerStarted()
    {
        // Keep a single timer alive so the greeting can update as the day changes.
        if (_greetingTimer is not null)
        {
            return;
        }

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return;
        }

        _greetingTimer = dispatcher.CreateTimer();
        _greetingTimer.Interval = TimeSpan.FromMinutes(1);
        _greetingTimer.Tick += (_, _) => UpdateGreetingAndDate();
        _greetingTimer.Start();
    }

    private async Task LoadHeavyNoteAsync()
    {
        // We set a guard so loading doesn't trigger a save.
        _isLoadingHeavyNote = true;
        try
        {
            var note = await _checkInStore.GetTodayHeavyNoteAsync();
            var updatedAt = await _checkInStore.GetTodayHeavyNoteUpdatedAtAsync();

            _heavyNoteText = note ?? string.Empty;
            _heavyNoteUpdatedAt = updatedAt;
            NotifyHeavyNoteDerived();
        }
        finally
        {
            _isLoadingHeavyNote = false;
        }
    }

    private void DebounceHeavyNoteSave()
    {
        // Cancel any in-flight save, then start a new one after a short pause.
        _heavyNoteSaveCts?.Cancel();
        _heavyNoteSaveCts = new CancellationTokenSource();
        var token = _heavyNoteSaveCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(600, token);
                await SaveHeavyNoteAsync();
            }
            catch (TaskCanceledException)
            {
                // If the user keeps typing, we cancel and restart the timer.
            }
        }, token);
    }

    private async Task SaveHeavyNoteAndCollapseAsync()
    {
        await SaveHeavyNoteAsync();
        IsHeavyNoteExpanded = false;
    }

    private async Task ClearHeavyNoteAsync()
    {
        HeavyNoteText = string.Empty;
        await SaveHeavyNoteAsync();
    }

    private async Task SaveHeavyNoteAsync()
    {
        // Save quietly in the background so the app feels calm.
        var now = DateTime.Now;
        await _checkInStore.UpsertTodayHeavyNoteAsync(_heavyNoteText, now);
        _heavyNoteUpdatedAt = string.IsNullOrWhiteSpace(_heavyNoteText) ? null : now;
        NotifyHeavyNoteDerived();
    }

    private void NotifyHeavyNoteDerived()
    {
        // These values depend on HeavyNoteText, so we refresh them together.
        RaisePropertyChanged(nameof(HeavyNotePreview));
        RaisePropertyChanged(nameof(HeavyNoteSavedLabel));
        RaisePropertyChanged(nameof(HasHeavyNote));
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
