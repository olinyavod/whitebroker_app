using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using AndroidWebView = Android.Webkit.WebView;

namespace WhiteBroker.Platforms.Android;

public class CustomWebViewHandler : WebViewHandler
{
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
            
            return webView;
        }
        
        throw new InvalidOperationException("Failed to create Android WebView");
    }
}

