using System.Windows.Input;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class CheckInViewModel : BaseViewModel
{
    private readonly ICheckInStore _checkInStore;
    private readonly INavigationService _navigationService;

    private int _energy = 3;
    private int _stress = 3;
    private int _focus = 3;
    private int _sleepQuality = 3;
    private string _note = string.Empty;
    private string _energyEmoji = "🙂";
    private string _stressEmoji = "😐";
    private string _focusEmoji = "🙂";
    private string _sleepEmoji = "😴";
    private bool _hasLoaded;

    public CheckInViewModel(ICheckInStore checkInStore, INavigationService navigationService)
    {
        _checkInStore = checkInStore;
        _navigationService = navigationService;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await _navigationService.GoBackAsync());

        Prompt = "How are you feeling right now?";

        UpdateEmojis();
    }

    public string Prompt { get; }

    public int Energy
    {
        get => _energy;
        set
        {
            SetProperty(ref _energy, value);
            UpdateEmojis();
        }
    }

    public int Stress
    {
        get => _stress;
        set
        {
            SetProperty(ref _stress, value);
            UpdateEmojis();
        }
    }

    public int Focus
    {
        get => _focus;
        set
        {
            SetProperty(ref _focus, value);
            UpdateEmojis();
        }
    }

    public int SleepQuality
    {
        get => _sleepQuality;
        set
        {
            SetProperty(ref _sleepQuality, value);
            UpdateEmojis();
        }
    }

    public string EnergyEmoji
    {
        get => _energyEmoji;
        private set => SetProperty(ref _energyEmoji, value);
    }

    public string StressEmoji
    {
        get => _stressEmoji;
        private set => SetProperty(ref _stressEmoji, value);
    }

    public string FocusEmoji
    {
        get => _focusEmoji;
        private set => SetProperty(ref _focusEmoji, value);
    }

    public string SleepEmoji
    {
        get => _sleepEmoji;
        private set => SetProperty(ref _sleepEmoji, value);
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public async Task LoadAsync()
    {
        if (_hasLoaded)
        {
            return;
        }

        // Load today's entry (if any) so the user can update rather than duplicate.
        var today = await _checkInStore.GetTodayAsync();
        if (today is null)
        {
            InitializeNewToday();
        }
        else
        {
            InitializeFromExistingToday(today);
        }

        _hasLoaded = true;
    }

    public void InitializeNewToday()
    {
        Energy = 3;
        Stress = 3;
        Focus = 3;
        SleepQuality = 3;
        Note = string.Empty;
        UpdateEmojis();
    }

    public void InitializeFromExistingToday(CheckIn today)
    {
        Energy = today.Energy;
        Stress = today.Stress;
        Focus = today.Focus;
        SleepQuality = today.SleepQuality;
        Note = today.Note ?? string.Empty;
        UpdateEmojis();
    }

    public async Task SaveAsync()
    {
        // Create a simple snapshot without any scoring or judgment.
        var checkIn = new CheckIn
        {
            Energy = Energy,
            Stress = Stress,
            Focus = Focus,
            SleepQuality = SleepQuality,
            Note = string.IsNullOrWhiteSpace(Note) ? null : Note
        };

        await _checkInStore.UpsertTodayAsync(checkIn);
        await _navigationService.GoBackAsync();
    }

    private void UpdateEmojis()
    {
        EnergyEmoji = Energy switch
        {
            <= 1 => "😴",
            2 => "🙂",
            3 => "😌",
            4 => "😊",
            _ => "⚡️"
        };

        StressEmoji = Stress switch
        {
            <= 1 => "😌",
            2 => "🙂",
            3 => "😐",
            4 => "😟",
            _ => "😫"
        };

        FocusEmoji = Focus switch
        {
            <= 1 => "😵",
            2 => "😕",
            3 => "🙂",
            4 => "😃",
            _ => "🎯"
        };

        SleepEmoji = SleepQuality switch
        {
            <= 1 => "🥱",
            2 => "😪",
            3 => "😴",
            4 => "😊",
            _ => "🌟"
        };
    }
}
