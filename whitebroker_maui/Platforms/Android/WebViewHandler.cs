using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WhiteBroker.Services;
using AndroidWebView = Android.Webkit.WebView;
using AndroidCookieManager = Android.Webkit.CookieManager;

namespace WhiteBroker.Platforms.Android;

public class CustomWebViewHandler : WebViewHandler
{
    private static Services.CookieManager? _cookieManager;

    public static void SetCookieManager(Services.CookieManager cookieManager)
    {
        _cookieManager = cookieManager;
    }

    protected override AndroidWebView CreatePlatformView()
    {
        var webView = base.CreatePlatformView();
        
        if (webView != null)
        {
            // Включаем JavaScript (уже включен по умолчанию в MAUI)
            webView.Settings.JavaScriptEnabled = true;
            
            // Включаем поддержку DOM Storage (для localStorage и sessionStorage)
            webView.Settings.DomStorageEnabled = true;
            
            // Включаем кэширование
            webView.Settings.CacheMode = CacheModes.Default;
            
            // Включаем поддержку встроенных видео
            webView.Settings.MediaPlaybackRequiresUserGesture = false;
            
            // Улучшаем поддержку масштабирования
            webView.Settings.UseWideViewPort = true;
            webView.Settings.LoadWithOverviewMode = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.DisplayZoomControls = false;
            
            // Улучшаем скроллинг при отображении клавиатуры
            webView.ScrollBarStyle = global::Android.Views.ScrollbarStyles.InsideOverlay;
            webView.VerticalScrollBarEnabled = true;
            
            // Разрешаем смешанный контент (если нужно)
            webView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;

            // Включаем сохранение куков
            var androidCookieManager = AndroidCookieManager.Instance;
            if (androidCookieManager != null)
            {
                androidCookieManager.SetAcceptCookie(true);
                androidCookieManager.SetAcceptThirdPartyCookies(webView, true);
            }

            // Загружаем сохраненные куки
            _ = LoadSavedCookiesAsync();
            
            return webView;
        }
        
        throw new InvalidOperationException("Failed to create Android WebView");
    }

    /// <summary>
    /// Загружает сохраненные куки и устанавливает их в WebView
    /// </summary>
    private async Task LoadSavedCookiesAsync()
    {
        if (_cookieManager == null)
            return;

        try
        {
            var cookies = await _cookieManager.LoadCookiesAsync();
            
            if (cookies.Count > 0)
            {
                var androidCookieManager = AndroidCookieManager.Instance;
                if (androidCookieManager != null)
                {
                    foreach (var cookie in cookies)
                    {
                        var cookieString = $"{cookie.Name}={cookie.Value}; Domain={cookie.Domain}; Path={cookie.Path}";
                        
                        if (cookie.ExpiresUnixTime > 0)
                        {
                            var expiresDate = cookie.GetExpiresDate();
                            if (expiresDate.HasValue)
                            {
                                cookieString += $"; Expires={expiresDate.Value:R}";
                            }
                        }
                        
                        if (cookie.Secure)
                        {
                            cookieString += "; Secure";
                        }
                        
                        if (cookie.HttpOnly)
                        {
                            cookieString += "; HttpOnly";
                        }

                        // Устанавливаем кук для домена
                        var url = $"https://{cookie.Domain.TrimStart('.')}";
                        androidCookieManager.SetCookie(url, cookieString);
                    }

                    // Принудительно сохраняем куки
                    androidCookieManager.Flush();

#if DEBUG
                    Console.WriteLine($"[Android WebView] Восстановлено {cookies.Count} куков");
#endif
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[Android WebView] Ошибка при загрузке куков: {ex.Message}");
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
            var androidCookieManager = AndroidCookieManager.Instance;
            if (androidCookieManager != null)
            {
                var url = $"https://{domain}";
                var cookieString = androidCookieManager.GetCookie(url);
                
                if (!string.IsNullOrEmpty(cookieString))
                {
                    var cookies = ParseCookieString(cookieString, domain);
                    await _cookieManager.SaveCookiesAsync(cookies);

#if DEBUG
                    Console.WriteLine($"[Android WebView] Сохранено {cookies.Count} куков для {domain}");
#endif
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[Android WebView] Ошибка при сохранении куков: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Парсит строку куков
    /// </summary>
    private static List<CookieData> ParseCookieString(string cookieString, string domain)
    {
        var cookies = new List<CookieData>();
        
        var pairs = cookieString.Split(';');
        foreach (var pair in pairs)
        {
            var trimmedPair = pair.Trim();
            var parts = trimmedPair.Split('=', 2);
            
            if (parts.Length == 2)
            {
                cookies.Add(new CookieData
                {
                    Name = parts[0].Trim(),
                    Value = parts[1].Trim(),
                    Domain = domain,
                    Path = "/",
                    ExpiresUnixTime = DateTimeOffset.Now.AddYears(1).ToUnixTimeSeconds(), // По умолчанию 1 год
                    Secure = true,
                    HttpOnly = false
                });
            }
        }
        
        return cookies;
    }
}

