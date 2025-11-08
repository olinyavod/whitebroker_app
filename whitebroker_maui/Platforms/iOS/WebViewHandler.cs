using System.Net;
using Foundation;
using UIKit;
using WebKit;
using Microsoft.Maui.Handlers;
using WhiteBroker.Services;

namespace WhiteBroker.Platforms.iOS;

public class CustomWebViewHandler : WebViewHandler
{
    private static Services.CookieManager? _cookieManager;

    public static void SetCookieManager(Services.CookieManager cookieManager)
    {
        _cookieManager = cookieManager;
    }

    protected override WKWebView CreatePlatformView()
    {
        var config = new WKWebViewConfiguration();
        
        // Настраиваем preferences для JavaScript
        var preferences = new WKWebpagePreferences();
        
        // Включаем поддержку JavaScript (iOS 14+)
        if (OperatingSystem.IsIOSVersionAtLeast(14, 0) || OperatingSystem.IsMacCatalystVersionAtLeast(14, 0))
        {
            preferences.AllowsContentJavaScript = true;
        }
        else
        {
            // Для более старых версий iOS используем устаревший API
#pragma warning disable CA1422
            config.Preferences.JavaScriptEnabled = true;
#pragma warning restore CA1422
        }
        
        config.DefaultWebpagePreferences = preferences;
        config.Preferences.JavaScriptCanOpenWindowsAutomatically = true;
        
        // Включаем поддержку inline видео
        config.AllowsInlineMediaPlayback = true;
        
        // Отключаем Picture-in-Picture для видео (опционально)
        config.AllowsPictureInPictureMediaPlayback = true;

        // Используем постоянное хранилище куков
        config.WebsiteDataStore = WKWebsiteDataStore.DefaultDataStore;
        
        var webView = new WKWebView(CoreGraphics.CGRect.Empty, config);
        
        // Настраиваем скроллинг
        webView.ScrollView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Automatic;
        webView.ScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;
        
        // Разрешаем увеличение
        webView.ScrollView.MaximumZoomScale = 3.0f;
        webView.ScrollView.MinimumZoomScale = 1.0f;

        // Загружаем сохраненные куки
        _ = LoadSavedCookiesAsync(config.WebsiteDataStore);
        
        return webView;
    }

    /// <summary>
    /// Загружает сохраненные куки и устанавливает их в WebView
    /// </summary>
    private async Task LoadSavedCookiesAsync(WKWebsiteDataStore dataStore)
    {
        if (_cookieManager == null)
            return;

        try
        {
            var cookies = await _cookieManager.LoadCookiesAsync();
            
            if (cookies.Count > 0)
            {
                var cookieStore = dataStore.HttpCookieStore;

                foreach (var cookie in cookies)
                {
                    var nsCookie = new NSHttpCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);

                    // Устанавливаем дату истечения, если она есть
                    if (cookie.ExpiresUnixTime > 0)
                    {
                        var expiresDate = cookie.GetExpiresDate();
                        if (expiresDate.HasValue)
                        {
	                        nsCookie = new NSHttpCookie(
		                        new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
	                        );
                        }
                    }

                    await cookieStore.SetCookieAsync(nsCookie);
                }

#if DEBUG
                Console.WriteLine($"[iOS WebView] Восстановлено {cookies.Count} куков");
#endif
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[iOS WebView] Ошибка при загрузке куков: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Сохраняет текущие куки из WebView
    /// </summary>
    public static async Task SaveCurrentCookiesAsync(string domain)
    {
        if (_cookieManager == null)
            return;

        try
        {
            var dataStore = WKWebsiteDataStore.DefaultDataStore;
            var cookieStore = dataStore.HttpCookieStore;
            
            var allCookies = await cookieStore.GetAllCookiesAsync();
            
            var cookies = new List<CookieData>();
            
            foreach (var nsCookie in allCookies)
            {
                // Фильтруем куки только для нужного домена
                if (nsCookie.Domain.Contains(domain) || domain.Contains(nsCookie.Domain))
                {
                    var cookieData = new CookieData
                    {
                        Name = nsCookie.Name,
                        Value = nsCookie.Value,
                        Domain = nsCookie.Domain,
                        Path = nsCookie.Path,
                        Secure = nsCookie.IsSecure,
                        HttpOnly = nsCookie.IsHttpOnly
                    };

                    // Устанавливаем дату истечения
                    if (nsCookie.ExpiresDate != null)
                    {
                        var expiresDate = DateTimeOffset.FromUnixTimeSeconds((long)nsCookie.ExpiresDate.SecondsSinceReferenceDate + 978307200); // Reference date: 2001-01-01
                        cookieData.SetExpiresDate(expiresDate);
                    }
                    else
                    {
                        // Сессионный кук - устанавливаем срок на 1 год
                        cookieData.SetExpiresDate(DateTimeOffset.Now.AddYears(1));
                    }

                    cookies.Add(cookieData);
                }
            }

            if (cookies.Count > 0)
            {
                await _cookieManager.SaveCookiesAsync(cookies);

#if DEBUG
                Console.WriteLine($"[iOS WebView] Сохранено {cookies.Count} куков для {domain}");
#endif
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[iOS WebView] Ошибка при сохранении куков: {ex.Message}");
#endif
        }
    }
}

