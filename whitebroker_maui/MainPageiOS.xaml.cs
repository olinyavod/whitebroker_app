using WhiteBroker.Services;
#if IOS
using WhiteBroker.Platforms.iOS;
#endif

namespace WhiteBroker;

public partial class MainPageiOS : ContentPage
{
	private const string TargetDomain = "wb.easy-prog.ru";

	public MainPageiOS()
	{
		InitializeComponent();
		webView.Navigated += OnNavigated;
	}

	private async void OnNavigated(object? sender, WebNavigatedEventArgs e)
	{
		if (e.Result == WebNavigationResult.Success)
		{
			errorLayout.IsVisible = false;
			await SaveCookiesAsync();
		}
		else
		{
			errorLabel.Text = "Ошибка соединения";
			errorLayout.IsVisible = true;
		}
	}

	private void OnRefreshClicked(object? sender, EventArgs e)
	{
		errorLayout.IsVisible = false;
		webView.Source = "https://my.whitebrokerdv.ru";
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_ = SaveCookiesAsync();
	}

	private async Task SaveCookiesAsync()
	{
		try
		{
#if IOS
			await CustomWebViewHandler.SaveCurrentCookiesAsync(TargetDomain);
#endif
		}
		catch (Exception ex)
		{
#if DEBUG
			Console.WriteLine($"[iOS] Ошибка при сохранении куков: {ex.Message}");
#endif
		}
	}
}
