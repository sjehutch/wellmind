using WellMind.ViewModels;

namespace WellMind.Views;

public partial class WelcomeModalPage : ContentPage
{
    public WelcomeModalPage(WelcomeModalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ------------------------------------------------------------
        // This is a tiny “fade in” so the screen feels calm and smooth.
        //
        // Without this: the page pops in instantly (kinda harsh).
        // With this: it gently appears (more “wellness app” vibe).
        // ------------------------------------------------------------

        // Start invisible...
        Opacity = 0;

        // ...then fade in quickly.
        // 250ms is fast enough to not feel slow, but still feels polished.
        await this.FadeTo(1, 250, Easing.CubicOut);
    }
}