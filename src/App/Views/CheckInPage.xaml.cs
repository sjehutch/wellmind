using WellMind.ViewModels;

namespace WellMind.Views;

public partial class CheckInPage : ContentPage
{
    public CheckInPage(CheckInViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CheckInViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }

}
