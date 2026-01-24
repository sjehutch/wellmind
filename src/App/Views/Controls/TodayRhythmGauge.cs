using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace WellMind.Views.Controls;

public sealed class TodayRhythmGauge : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(TodayRhythmGauge),
        0d,
        propertyChanged: OnValueChanged);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(TodayRhythmGauge),
        Colors.Green,
        propertyChanged: OnColorChanged);

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(TodayRhythmGauge),
        Colors.LightGray,
        propertyChanged: OnColorChanged);

    private readonly GraphicsView _graphicsView;
    private readonly GaugeDrawable _drawable;
    private double _animatedValue;
    private float _pulse;

    private bool _isLoaded;

    public TodayRhythmGauge()
    {
        _drawable = new GaugeDrawable(() => (float)_animatedValue, () => AccentColor, () => TrackColor, () => _pulse);
        _graphicsView = new GraphicsView
        {
            Drawable = _drawable,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        _graphicsView.SetBinding(WidthRequestProperty, new Binding(nameof(WidthRequest), source: this));
        _graphicsView.SetBinding(HeightRequestProperty, new Binding(nameof(HeightRequest), source: this));

        Content = _graphicsView;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TodayRhythmGauge)bindable;
        var value = (double)newValue;

        if (!control._isLoaded || control.Handler is null)
        {
            control._animatedValue = Math.Clamp(value, 0, 1);
            control._graphicsView.Invalidate();
            return;
        }

        control.AnimateTo(value);
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TodayRhythmGauge)bindable;
        control._graphicsView.Invalidate();
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _isLoaded = true;
        AnimateTo(Value);
        StartPulse();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _isLoaded = false;
        this.AbortAnimation("GaugeSweep");
        this.AbortAnimation("GaugePulse");
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        _graphicsView.Invalidate();

        if (_isLoaded && Handler is not null)
        {
            AnimateTo(Value);
            StartPulse();
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _graphicsView.Invalidate();
    }

    private void AnimateTo(double target)
    {
        if (Handler is null)
        {
            _animatedValue = Math.Clamp(target, 0, 1);
            _graphicsView.Invalidate();
            return;
        }

        target = Math.Clamp(target, 0, 1);
        var start = _animatedValue;

        this.AbortAnimation("GaugeSweep");
        var animation = new Animation(
            v =>
            {
                _animatedValue = v;
                _graphicsView.Invalidate();
            },
            start,
            target,
            Easing.CubicOut);

        animation.Commit(this, "GaugeSweep", 16, 700);
    }

    private void StartPulse()
    {
        this.AbortAnimation("GaugePulse");
        var animation = new Animation(
            v =>
            {
                _pulse = (float)v;
                _graphicsView.Invalidate();
            },
            0.15,
            0.6,
            Easing.SinInOut);

        animation.Commit(this, "GaugePulse", 32, 1600, repeat: () => true);
    }

    private sealed class GaugeDrawable : IDrawable
    {
        private readonly Func<float> _getValue;
        private readonly Func<Color> _getAccentColor;
        private readonly Func<Color> _getTrackColor;
        private readonly Func<float> _getPulse;

        public GaugeDrawable(
            Func<float> getValue,
            Func<Color> getAccentColor,
            Func<Color> getTrackColor,
            Func<float> getPulse)
        {
            _getValue = getValue;
            _getAccentColor = getAccentColor;
            _getTrackColor = getTrackColor;
            _getPulse = getPulse;
        }

        public void Draw(ICanvas canvas, RectF rect)
        {
            var size = Math.Min(rect.Width, rect.Height);
            var stroke = size * 0.12f;
            var radius = (size / 2) - stroke;
            var center = rect.Center;

            var startAngle = 135f;
            var sweepAngle = 270f;
            var value = Math.Clamp(_getValue(), 0, 1);

            canvas.StrokeSize = stroke;
            canvas.StrokeColor = _getTrackColor();
            canvas.DrawArc(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2,
                startAngle,
                sweepAngle,
                false,
                false);

            canvas.StrokeColor = _getAccentColor();
            canvas.DrawArc(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2,
                startAngle,
                sweepAngle * value,
                false,
                false);

            var angle = MathF.PI * (startAngle + (sweepAngle * value)) / 180f;
            var dotX = center.X + (radius * MathF.Cos(angle));
            var dotY = center.Y + (radius * MathF.Sin(angle));

            canvas.FillColor = _getAccentColor().WithAlpha(0.95f);
            canvas.FillCircle(dotX, dotY, stroke * 0.55f);

            var pulse = _getPulse();
            canvas.StrokeColor = _getAccentColor().WithAlpha(0.25f);
            canvas.StrokeSize = stroke * 0.3f;
            canvas.DrawCircle(dotX, dotY, stroke * (0.7f + pulse));
        }
    }
}
