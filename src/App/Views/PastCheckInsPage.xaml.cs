using WellMind.ViewModels;

namespace WellMind.Views;

public partial class PastCheckInsPage : ContentPage
{
    public PastCheckInsPage(PastCheckInsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PastCheckInsViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
