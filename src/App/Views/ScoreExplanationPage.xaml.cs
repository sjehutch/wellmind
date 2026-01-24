using WellMind.ViewModels;

namespace WellMind.Views;

public partial class ScoreExplanationPage : ContentPage
{
    public ScoreExplanationPage(ScoreExplanationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
