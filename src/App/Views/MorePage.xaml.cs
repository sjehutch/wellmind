using WellMind.Features.Support.TalkToSomeone;
using WellMind.Pages;

namespace WellMind.Views;

public partial class MorePage : ContentPage
{
    public MorePage()
    {
        InitializeComponent();
    }

    private async void OnPrivacyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PrivacyCommitmentPage));
    }

    private async void OnScoreClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ScoreExplanationPage));
    }

    private async void OnGroundedClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GroundedPage));
    }

    private async void OnTalkToSomeoneClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TalkToSomeonePage));
    }
}
