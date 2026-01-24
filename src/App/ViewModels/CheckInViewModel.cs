using System.Windows.Input;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class CheckInViewModel : BaseViewModel
{
    private readonly ICheckInService _checkInService;
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

    public CheckInViewModel(ICheckInService checkInService, INavigationService navigationService)
    {
        _checkInService = checkInService;
        _navigationService = navigationService;

        SubmitCommand = new Command(async () => await SubmitAsync());
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

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task SubmitAsync()
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

        await _checkInService.SaveAsync(checkIn);
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
