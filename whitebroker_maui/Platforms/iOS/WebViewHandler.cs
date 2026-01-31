using Foundation;
using Microsoft.Maui.Handlers;
using WebKit;
using WhiteBroker.Services;

namespace WhiteBroker.Platforms.iOS;

public class CustomWebViewHandler : WebViewHandler
{
    private static Services.CookieManager? _cookieManager;

    public static void SetCookieManager(Services.CookieManager cookieManager)
    {
        _cookieManager = cookieManager;
    }

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

                    if (nsCookie.ExpiresDate != null)
                    {
                        var expiresDate = DateTimeOffset.FromUnixTimeSeconds(
                            (long)nsCookie.ExpiresDate.SecondsSinceReferenceDate + 978307200);
                        cookieData.SetExpiresDate(expiresDate);
                    }
                    else
                    {
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
