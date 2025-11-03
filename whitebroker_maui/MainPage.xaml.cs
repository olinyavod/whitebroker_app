namespace WhiteBroker;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		// Подписываемся на события WebView
		webView.Navigating += OnNavigating;
		webView.Navigated += OnNavigated;
	}

	private void OnNavigating(object? sender, WebNavigatingEventArgs e)
	{
		// Показываем индикатор загрузки
		loadingIndicator.IsVisible = true;
		loadingIndicator.IsRunning = true;
		errorLayout.IsVisible = false;
	}

	private void OnNavigated(object? sender, WebNavigatedEventArgs e)
	{
		// Скрываем индикатор загрузки
		loadingIndicator.IsVisible = false;
		loadingIndicator.IsRunning = false;

		// Проверяем статус навигации
		if (e.Result != WebNavigationResult.Success)
		{
			ShowError($"Не удалось загрузить страницу. Статус: {e.Result}");
		}
	}

	private void ShowError(string message)
	{
		errorLabel.Text = message;
		errorLayout.IsVisible = true;
		webView.IsVisible = false;
	}

	private void OnRefreshClicked(object? sender, EventArgs e)
	{
		errorLayout.IsVisible = false;
		webView.IsVisible = true;
		webView.Reload();
	}
}





