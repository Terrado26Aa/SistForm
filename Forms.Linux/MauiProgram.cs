using CommunityToolkit.Maui;
using Forms.Services;
using Forms.Views;
using Forms.ViewModels;
using Microsoft.Extensions.Logging;

namespace Forms.Linux;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiAppLinuxGtk4<App>()
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

        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<Login>();

        return builder.Build();
    }
}
