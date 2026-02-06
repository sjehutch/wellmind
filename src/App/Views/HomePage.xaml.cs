using WellMind.ViewModels;

namespace WellMind.Views;

public partial class HomePage : ContentPage
{
    private CancellationTokenSource? _readingHintCts;
    private static bool _hasShownReadingHint;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load data as you already do
        if (BindingContext is HomeViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }

        // Start the subtle "hey you can tap this" hint animation
        StartReadingHintAnimation();
    }

    protected override void OnDisappearing()
    {
        StopReadingHintAnimation();
        base.OnDisappearing();
    }

    private void StartReadingHintAnimation()
    {
        StopReadingHintAnimation();

        if (_hasShownReadingHint)
        {
            return;
        }

        // Safety: if the XAML element isn't present, do nothing
        if (ReadingHintLabel is null)
        {
            return;
        }

        _readingHintCts = new CancellationTokenSource();
        _ = RunReadingHintAnimationAsync(_readingHintCts.Token);
    }

    private void StopReadingHintAnimation()
    {
        _readingHintCts?.Cancel();
        _readingHintCts = null;
    }

    private async Task RunReadingHintAnimationAsync(CancellationToken token)
    {
        // Wait a moment so it doesn't animate instantly on load
        await Task.Delay(800, token);

        for (var i = 0; i < 3 && !token.IsCancellationRequested; i++)
        {
            await ReadingHintLabel.FadeTo(0.5, 300, Easing.CubicOut);
            await ReadingHintLabel.FadeTo(1.0, 300, Easing.CubicIn);
            await Task.Delay(900, token);
        }

        _hasShownReadingHint = true;
    }
}
