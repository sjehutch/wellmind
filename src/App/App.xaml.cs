using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching;
using WellMind.Services;
using WellMind.Views;

namespace WellMind;

public partial class App : Application
{
    private readonly ILoggerService _logger;
    private readonly IServiceProvider _services;
    private readonly IFirstRunStore _firstRunStore;
    private readonly SemaphoreSlim _launchModalGate = new(1, 1);
    private bool _hasRunLaunchModalSequence;

    public App(IServiceProvider services, ILoggerService logger, IFirstRunStore firstRunStore)
    {
        InitializeComponent();
        _logger = logger;
        _services = services;
        _firstRunStore = firstRunStore;
        RegisterExceptionHandlers();
        _logger.LogInfo("App started.");
        try
        {
            MainPage = services.GetRequiredService<LaunchPage>();
        }
        catch (Exception exception)
        {
            _logger.LogException(exception, "Failed to create LaunchPage");
            throw;
        }

    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        MainThread.BeginInvokeOnMainThread(() => _ = RunLaunchModalSequenceAsync());
        return window;
    }

    private async Task RunLaunchModalSequenceAsync()
    {
        await _launchModalGate.WaitAsync();
        try
        {
            if (_hasRunLaunchModalSequence)
            {
                return;
            }

            var rootPage = await WaitForRootPageAsync();
            if (rootPage is null)
            {
                _logger.LogInfo("Skipped launch history reminder because root page was unavailable.");
                return;
            }

            if (!_firstRunStore.HasSeenWelcome())
            {
                var welcome = _services.GetRequiredService<WelcomeModalPage>();
                var closedTcs = new TaskCompletionSource<bool>();
                void HandleDisappearing(object? sender, EventArgs args)
                {
                    welcome.Disappearing -= HandleDisappearing;
                    closedTcs.TrySetResult(true);
                }

                welcome.Disappearing += HandleDisappearing;
                await rootPage.Navigation.PushModalAsync(welcome);
                await closedTcs.Task;
            }

            try
            {
                var history = _services.GetRequiredService<HistoryReminderModalPage>();
                await rootPage.Navigation.PushModalAsync(history);
                _hasRunLaunchModalSequence = true;
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "Failed to show HistoryReminderModalPage");
            }
        }
        finally
        {
            _launchModalGate.Release();
        }
    }

    private static async Task<Page?> WaitForRootPageAsync()
    {
        for (var i = 0; i < 20; i++)
        {
            var page = Current?.Windows.FirstOrDefault()?.Page;
            if (page is not null)
            {
                return page;
            }

            await Task.Delay(100);
        }

        return null;
    }

    public void ShowHomeShellFromLaunch()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                MainPage = _services.GetRequiredService<AppShell>();
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "Failed to create AppShell");
                throw;
            }
        });
    }

    private void RegisterExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                _logger.LogException(exception, "AppDomain unhandled exception");
                return;
            }

            _logger.LogInfo($"AppDomain unhandled exception: {args.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            _logger.LogException(args.Exception, "Unobserved task exception");
            args.SetObserved();
        };

        // MAUI doesn't expose a cross-platform dispatcher exception event.
    }
}
