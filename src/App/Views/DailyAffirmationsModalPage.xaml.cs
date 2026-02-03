using WellMind.ViewModels;

namespace WellMind.Views;

public partial class DailyAffirmationsModalPage : ContentPage
{
    public DailyAffirmationsModalPage(DailyAffirmationsModalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
