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

    public CheckInViewModel(ICheckInService checkInService, INavigationService navigationService)
    {
        _checkInService = checkInService;
        _navigationService = navigationService;

        SubmitCommand = new Command(async () => await SubmitAsync());
        CancelCommand = new Command(async () => await _navigationService.GoBackAsync());

        Prompt = "How are you feeling right now?";
    }

    public string Prompt { get; }

    public int Energy
    {
        get => _energy;
        set => SetProperty(ref _energy, value);
    }

    public int Stress
    {
        get => _stress;
        set => SetProperty(ref _stress, value);
    }

    public int Focus
    {
        get => _focus;
        set => SetProperty(ref _focus, value);
    }

    public int SleepQuality
    {
        get => _sleepQuality;
        set => SetProperty(ref _sleepQuality, value);
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
}
