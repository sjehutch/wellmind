using Microsoft.Extensions.Logging;
using WellMind.Features.Support.TalkToSomeone;
using WellMind.Pages;
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
		builder.Services.AddTransient<WelcomeModalPage>();
		builder.Services.AddTransient<LaunchPage>();
		builder.Services.AddTransient<DailyAffirmationsModalPage>();
		builder.Services.AddTransient<HistoryReminderModalPage>();
		builder.Services.AddTransient<PastCheckInsPage>();
		builder.Services.AddTransient<ScoreExplanationPage>();
		builder.Services.AddTransient<PrivacyCommitmentPage>();
		builder.Services.AddTransient<GroundedPage>();
		builder.Services.AddTransient<ReadingListPage>();
		builder.Services.AddTransient<TalkToSomeonePage>();
		builder.Services.AddTransient<GentleReminderPage>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<CheckInViewModel>();
		builder.Services.AddTransient<WeeklyTrendsViewModel>();
		builder.Services.AddTransient<InAppBrowserViewModel>();
		builder.Services.AddTransient<WelcomeModalViewModel>();
		builder.Services.AddTransient<DailyAffirmationsModalViewModel>();
		builder.Services.AddTransient<HistoryReminderModalViewModel>();
		builder.Services.AddTransient<PastCheckInsViewModel>();
		builder.Services.AddTransient<ScoreExplanationViewModel>();
		builder.Services.AddTransient<PrivacyCommitmentViewModel>();
		builder.Services.AddTransient<GroundedViewModel>();
		builder.Services.AddTransient<TalkToSomeoneViewModel>();
		builder.Services.AddTransient<GentleReminderViewModel>();

		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
		builder.Services.AddSingleton<ICheckInStore, JsonFileCheckInStore>();
		builder.Services.AddSingleton<ITrendService, InMemoryTrendService>();
		builder.Services.AddSingleton<ITipService, InMemoryTipService>();
		builder.Services.AddSingleton<IResourceLinkService, InMemoryResourceLinkService>();
		builder.Services.AddSingleton<IExternalLinkService, ExternalLinkService>();
		builder.Services.AddSingleton<IGrounded, GroundedService>();
		builder.Services.AddSingleton<IEnergyWindowsService, EnergyWindowsService>();
		builder.Services.AddSingleton<IFirstRunStore, FirstRunStore>();
		builder.Services.AddSingleton<IHomeBackgroundService, HomeBackgroundService>();
		builder.Services.AddSingleton<TalkToSomeoneConfigLoader>();
		builder.Services.AddSingleton<ILoggerService, FileLoggerService>();
		builder.Services.AddSingleton<IReminderSettingsStore, ReminderSettingsStore>();
		builder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
		builder.Services.AddSingleton<IGratitudeReminderService, GratitudeReminderService>();
		builder.Services.AddSingleton<HistoryReminderService>();
		builder.Services.AddSingleton<ITonePackStore, TonePackStore>();
		builder.Services.AddSingleton<ITipFeedbackStore, TipFeedbackStore>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
