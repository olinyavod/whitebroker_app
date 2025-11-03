using Microsoft.Extensions.Logging;

#if ANDROID
using WhiteBroker.Platforms.Android;
#elif IOS
using WhiteBroker.Platforms.iOS;
#endif

namespace WhiteBroker;

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
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				// Регистрируем кастомный обработчик WebView для Android
				handlers.AddHandler(typeof(WebView), typeof(CustomWebViewHandler));
#elif IOS
				// Регистрируем кастомный обработчик WebView для iOS
				handlers.AddHandler(typeof(WebView), typeof(CustomWebViewHandler));
#endif
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}





