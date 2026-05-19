using CommunityToolkit.Maui;

namespace Tsubaki;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                fonts.AddFont("Poppins-BoldItalic.ttf", "PoppinsBoldItalic");
                fonts.AddFont("Poppins-ExtraBold.ttf", "PoppinsExtraBold");
                fonts.AddFont("Poppins-ExtraBoldItalic.ttf", "PoppinsExtraBoldItalic");
                fonts.AddFont("Poppins-Italic.ttf", "PoppinsItalic");
                fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                fonts.AddFont("Poppins-SemiBoldItalic.ttf", "PoppinsSemiBoldItalic");
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Add Services
        builder.Services.AddTransient<RenderClients>();

        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddTransient<DeploysViewModel>();

        return builder;
    }
}
