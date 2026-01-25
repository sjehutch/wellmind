using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace WellMind.Views.Controls;

public sealed class ScoreDots : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(ScoreDots),
        1d,
        propertyChanged: OnValueChanged);

    public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
        nameof(FillColor),
        typeof(Color),
        typeof(ScoreDots),
        Colors.Green,
        propertyChanged: OnColorChanged);

    public static readonly BindableProperty EmptyColorProperty = BindableProperty.Create(
        nameof(EmptyColor),
        typeof(Color),
        typeof(ScoreDots),
        Colors.Gray,
        propertyChanged: OnColorChanged);

    private readonly List<BoxView> _dots = new();
    private bool _isLoaded;
    private int _animationVersion;

    public ScoreDots()
    {
        var layout = new HorizontalStackLayout
        {
            Spacing = 6,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };

        for (var i = 0; i < 5; i++)
        {
            var dot = new BoxView
            {
                WidthRequest = 8,
                HeightRequest = 8,
                CornerRadius = 4,
                Opacity = 0
            };
            _dots.Add(dot);
            layout.Children.Add(dot);
        }

        Content = layout;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public Color EmptyColor
    {
        get => (Color)GetValue(EmptyColorProperty);
        set => SetValue(EmptyColorProperty, value);
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScoreDots)bindable;
        control.UpdateDots(control._isLoaded);
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScoreDots)bindable;
        control.UpdateDots(false);
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _isLoaded = true;
        UpdateDots(true);
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _isLoaded = false;
        _animationVersion++;
    }

    private void UpdateDots(bool animate)
    {
        var filledCount = (int)Math.Round(Value, MidpointRounding.AwayFromZero);
        filledCount = Math.Clamp(filledCount, 1, 5);

        if (!animate)
        {
            ApplyDotState(filledCount);
            return;
        }

        _ = AnimateDotsAsync(filledCount);
    }

    private void ApplyDotState(int filledCount)
    {
        for (var i = 0; i < _dots.Count; i++)
        {
            var dot = _dots[i];
            dot.Color = i < filledCount ? FillColor : EmptyColor;
            dot.Opacity = 1;
            dot.Scale = 1;
        }
    }

    private async Task AnimateDotsAsync(int filledCount)
    {
        var version = ++_animationVersion;

        for (var i = 0; i < _dots.Count; i++)
        {
            if (version != _animationVersion)
            {
                return;
            }

            var dot = _dots[i];
            dot.Color = i < filledCount ? FillColor : EmptyColor;
            dot.Opacity = 0;
            dot.Scale = 0.8;

            await Task.WhenAll(
                dot.FadeTo(1, 120, Easing.CubicOut),
                dot.ScaleTo(1, 120, Easing.CubicOut));

            await Task.Delay(40);
        }
    }
}
