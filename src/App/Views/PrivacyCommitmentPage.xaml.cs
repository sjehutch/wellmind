using WellMind.ViewModels;

namespace WellMind.Views;

public partial class PrivacyCommitmentPage : ContentPage
{
    public PrivacyCommitmentPage(PrivacyCommitmentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
