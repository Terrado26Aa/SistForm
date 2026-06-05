using Forms.Services;
using Forms.Views;
using Forms.ViewModels;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace Forms;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiMaps()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		

#if DEBUG
		builder.Logging.AddDebug();
#endif
        // Servicios
        builder.Services.AddSingleton<ApiService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();

        // Views
        builder.Services.AddTransient<Login>();
        
        return builder.Build();
	}
}
