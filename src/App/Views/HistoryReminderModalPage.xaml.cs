using WellMind.ViewModels;

namespace WellMind.Views;

public partial class HistoryReminderModalPage : ContentPage
{
    public HistoryReminderModalPage(HistoryReminderModalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HistoryReminderModalViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
