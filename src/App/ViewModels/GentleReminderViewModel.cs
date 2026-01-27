using System.Windows.Input;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class GentleReminderViewModel : BaseViewModel
{
    // These helpers keep the page small and calm.
    private readonly IReminderSettingsStore _settingsStore;
    private readonly IGratitudeReminderService _reminderService;
    private readonly INavigationService _navigationService;
    private bool _isEnabled;
    private TimeSpan _reminderTime = new(9, 0, 0);
    private bool _isLoading;

    public GentleReminderViewModel(
        IReminderSettingsStore settingsStore,
        IGratitudeReminderService reminderService,
        INavigationService navigationService)
    {
        _settingsStore = settingsStore;
        _reminderService = reminderService;
        _navigationService = navigationService;

        CloseCommand = new Command(async () => await _navigationService.CloseModalAsync());
        SaveAndCloseCommand = new Command(async () => await SaveAndCloseAsync());
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value == _isEnabled)
            {
                return;
            }

            SetProperty(ref _isEnabled, value);

            // When the toggle changes, save and schedule quietly.
            if (!_isLoading)
            {
                _ = SaveAndApplyAsync();
            }
        }
    }

    public TimeSpan ReminderTime
    {
        get => _reminderTime;
        set
        {
            if (value == _reminderTime)
            {
                return;
            }

            SetProperty(ref _reminderTime, value);

            // When the time changes, save and reschedule quietly.
            if (!_isLoading)
            {
                _ = SaveAndApplyAsync();
            }
        }
    }

    public ICommand CloseCommand { get; }
    public ICommand SaveAndCloseCommand { get; }

    public async Task LoadAsync()
    {
        // Load saved settings without triggering schedule calls.
        _isLoading = true;
        try
        {
            IsEnabled = await _settingsStore.GetIsEnabledAsync();
            ReminderTime = await _settingsStore.GetTimeAsync();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SaveAndApplyAsync()
    {
        // First save locally so the app can remember the user's choice.
        await _settingsStore.SaveAsync(IsEnabled, ReminderTime);
        // Then schedule or cancel the daily reminder.
        await _reminderService.ApplySettingsAsync(IsEnabled, ReminderTime);
    }

    private async Task SaveAndCloseAsync()
    {
        // Save once more so the button feels explicit, then close the modal.
        await SaveAndApplyAsync();
        await _navigationService.CloseModalAsync();
    }
}
