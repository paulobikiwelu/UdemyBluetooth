using Microsoft.Extensions.Logging;
using Shiny.Infrastructure;

namespace UdemyBluetooth;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		RegisterShiny(builder.Services);

		return builder.Build();
	}

	static void RegisterShiny(IServiceCollection s)
	{
		s.AddShinyCoreServices();
		s.AddBluetoothLE();
		s.AddBluetoothLeHosting();
	}
}
