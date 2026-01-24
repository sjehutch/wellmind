using WellMind.ViewModels;

namespace WellMind.Views;

public partial class CheckInPage : ContentPage
{
    private int _lastEnergyValue = 3;
    private int _lastStressValue = 3;
    private int _lastFocusValue = 3;
    private int _lastSleepValue = 3;

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

    private async void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (sender is not Slider slider)
        {
            return;
        }

        var nextValue = (int)Math.Round(e.NewValue);
        var (emojiLabel, lastValue) = GetEmojiTarget(slider);
        if (emojiLabel is null)
        {
            return;
        }

        if (nextValue != lastValue)
        {
            SetLastValue(slider, nextValue);
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await PulseEmojiAsync(emojiLabel);
        }
    }

    private void OnSliderDragStarted(object? sender, EventArgs e)
    {
        if (sender is not Slider slider)
        {
            return;
        }

        slider.Shadow = new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#6B7C8F")),
            Opacity = 0.35f,
            Radius = 12,
            Offset = new Point(0, 2)
        };
    }

    private void OnSliderDragCompleted(object? sender, EventArgs e)
    {
        if (sender is Slider slider)
        {
            slider.ClearValue(VisualElement.ShadowProperty);
        }
    }

    private async Task PulseEmojiAsync(VisualElement target)
    {
        target.Opacity = 0.7;
        await target.ScaleTo(1.12, 120, Easing.CubicOut);
        await target.ScaleTo(1.0, 140, Easing.CubicIn);
        await target.FadeTo(1.0, 120, Easing.CubicIn);
    }

    private (Label? label, int lastValue) GetEmojiTarget(Slider slider)
    {
        if (slider == EnergySlider)
        {
            return (EnergyEmojiLabel, _lastEnergyValue);
        }

        if (slider == StressSlider)
        {
            return (StressEmojiLabel, _lastStressValue);
        }

        if (slider == FocusSlider)
        {
            return (FocusEmojiLabel, _lastFocusValue);
        }

        if (slider == SleepSlider)
        {
            return (SleepEmojiLabel, _lastSleepValue);
        }

        return (null, 0);
    }

    private void SetLastValue(Slider slider, int value)
    {
        if (slider == EnergySlider)
        {
            _lastEnergyValue = value;
        }
        else if (slider == StressSlider)
        {
            _lastStressValue = value;
        }
        else if (slider == FocusSlider)
        {
            _lastFocusValue = value;
        }
        else if (slider == SleepSlider)
        {
            _lastSleepValue = value;
        }
    }
}
