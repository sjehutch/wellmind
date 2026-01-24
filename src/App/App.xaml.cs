using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WellMind.Services;

namespace WellMind;

public partial class App : Application
{
    private readonly ILoggerService _logger;

    public App(IServiceProvider services, ILoggerService logger)
    {
        InitializeComponent();
        _logger = logger;
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
