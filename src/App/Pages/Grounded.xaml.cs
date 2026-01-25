using WellMind.ViewModels;

namespace WellMind.Pages;

public partial class GroundedPage : ContentPage
{
    public GroundedPage(GroundedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is GroundedViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
