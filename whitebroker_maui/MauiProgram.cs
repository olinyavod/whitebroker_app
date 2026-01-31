using Microsoft.Extensions.Logging;
using WhiteBroker.Services;

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
#endif
			});

		// Регистрируем CookieManager как singleton
		builder.Services.AddSingleton<CookieManager>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Инициализируем CookieManager в обработчиках WebView
		var cookieManager = app.Services.GetRequiredService<CookieManager>();
#if ANDROID
		CustomWebViewHandler.SetCookieManager(cookieManager);
#elif IOS
		CustomWebViewHandler.SetCookieManager(cookieManager);
#endif

		return app;
	}
}





