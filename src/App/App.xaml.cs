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
            MainPage = services.GetRequiredService<AppShell>();
        }
        catch (Exception exception)
        {
            _logger.LogException(exception, "Failed to create AppShell");
            throw;
        }

        ShowWelcomeIfNeeded();
    }

    private void ShowWelcomeIfNeeded()
    {
        if (_firstRunStore.HasSeenWelcome())
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var modal = _services.GetRequiredService<WelcomeModalPage>();
            await MainPage.Navigation.PushModalAsync(modal);
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
