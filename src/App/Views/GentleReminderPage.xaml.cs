using WellMind.ViewModels;

namespace WellMind.Views;

public partial class GentleReminderPage : ContentPage
{
    private readonly GentleReminderViewModel _viewModel;

    public GentleReminderPage(GentleReminderViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Load saved settings so the switch and time are correct.
        await _viewModel.LoadAsync();
    }
}
