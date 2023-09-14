using SkiaSharp.Views.Maui.Controls.Hosting;
using Microsoft.Extensions.Logging;
using BeerBrewerApp.Services;

namespace BeerBrewerApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseSkiaSharp(true)
			.UseMauiApp<App>()
			.RegisterServices()
			.RegisterViewModels()
			.RegisterViews()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddTransient<Views.AutomationView>();
		mauiAppBuilder.Services.AddTransient<Views.MonitorView>();
        mauiAppBuilder.Services.AddTransient<Views.SettingsView>();
        return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<ViewModels.MonitorViewModel>();
		mauiAppBuilder.Services.AddSingleton<ViewModels.AutomationViewModel>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IAlertService, AlertService>();
		mauiAppBuilder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
		mauiAppBuilder.Services.AddSingleton<IMicroControllerInterface, MicroControllerSpoofer>();
		mauiAppBuilder.Services.AddSingleton<ICommandService, CommandService>();
		mauiAppBuilder.Services.AddSingleton<IAutomationService, AutomationService>();
		return mauiAppBuilder;
	}
}
