using Microsoft.Extensions.Logging;
using WellMind.Services;
using WellMind.ViewModels;
using WellMind.Views;

namespace WellMind;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register UI and services for simple constructor injection.
		builder.Services.AddSingleton<AppShell>();

		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<CheckInPage>();
		builder.Services.AddTransient<InAppBrowserPage>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<CheckInViewModel>();
		builder.Services.AddTransient<WeeklyTrendsViewModel>();
		builder.Services.AddTransient<InAppBrowserViewModel>();

		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
		builder.Services.AddSingleton<ICheckInService, InMemoryCheckInService>();
		builder.Services.AddSingleton<ITrendService, InMemoryTrendService>();
		builder.Services.AddSingleton<ITipService, InMemoryTipService>();
		builder.Services.AddSingleton<IResourceLinkService, InMemoryResourceLinkService>();
		builder.Services.AddSingleton<IExternalLinkService, ExternalLinkService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
