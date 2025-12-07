using Forms.Services;
using Forms.Views;
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
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		

#if DEBUG
		builder.Logging.AddDebug();
#endif
        //Registramos el servicio y la pagina de login
        //AddSingleton mantiene la misma instancia de ApiService durante toda la app
        builder.Services.AddSingleton<ApiService>();
        //AddTransient crea una nueva instancia de Login cada vez que se solicita
        builder.Services.AddTransient<Login>();
        return builder.Build();
	}
}
