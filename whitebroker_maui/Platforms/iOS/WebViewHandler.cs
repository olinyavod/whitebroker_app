using Foundation;
using UIKit;
using WebKit;
using Microsoft.Maui.Handlers;

namespace WhiteBroker.Platforms.iOS;

public class CustomWebViewHandler : WebViewHandler
{
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
        
        var webView = new WKWebView(CoreGraphics.CGRect.Empty, config);
        
        // Настраиваем скроллинг
        webView.ScrollView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Automatic;
        webView.ScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;
        
        // Разрешаем увеличение
        webView.ScrollView.MaximumZoomScale = 3.0f;
        webView.ScrollView.MinimumZoomScale = 1.0f;
        
        return webView;
    }
}

