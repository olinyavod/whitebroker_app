using System.Text;
using WhiteBroker.Services;
#if ANDROID
using WhiteBroker.Platforms.Android;
#elif IOS
using WhiteBroker.Platforms.iOS;
#endif

namespace WhiteBroker;

public partial class MainPage : ContentPage
{
	private const string TargetDomain = "wb.easy-prog.ru";
	private readonly CookieManager _cookieManager;
	private System.Timers.Timer? _cookieSaveTimer;

#if DEBUG
	private bool isConsoleVisible = false;
	private List<ConsoleMessage> consoleMessages = new List<ConsoleMessage>();
	private string lastMessage = "";
	private int messageCount = 0;
	private Label? lastMessageLabel = null;
#endif

	public MainPage(CookieManager cookieManager)
	{
		InitializeComponent();
		
		_cookieManager = cookieManager;

#if DEBUG
		// Консоль доступна только в Debug режиме
		consoleToggleButton.IsVisible = true;
		AddConsoleMessage("info", "[DEBUG MODE] Console debugging enabled");
		AddConsoleMessage("info", "[Cookie Manager] Initialized");
#else
		// В Release режиме скрываем кнопку консоли
		consoleToggleButton.IsVisible = false;
		consolePanel.IsVisible = false;
#endif

		// Подписываемся на события WebView один раз
		webView.Navigating += OnWebViewNavigating;
		webView.Navigated += OnNavigated;

		// Настраиваем таймер для периодического сохранения куков (каждые 30 секунд)
		_cookieSaveTimer = new System.Timers.Timer(30000); // 30 секунд
		_cookieSaveTimer.Elapsed += async (sender, e) => await SaveCookiesAsync();
		_cookieSaveTimer.Start();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		
		// Сохраняем куки при закрытии страницы
		_ = SaveCookiesAsync();
		
		// Останавливаем таймер
		_cookieSaveTimer?.Stop();
	}

	/// <summary>
	/// Сохраняет текущие куки
	/// </summary>
	private async Task SaveCookiesAsync()
	{
		try
		{
#if ANDROID
			await CustomWebViewHandler.SaveCurrentCookiesAsync(TargetDomain);
#elif IOS
			await CustomWebViewHandler.SaveCurrentCookiesAsync(TargetDomain);
#endif

#if DEBUG
			AddConsoleMessage("info", "[Cookie Manager] Куки сохранены");
#endif
		}
		catch (Exception ex)
		{
#if DEBUG
			AddConsoleMessage("error", $"[Cookie Manager] Ошибка при сохранении куков: {ex.Message}");
#endif
		}
	}

	// Объединённый обработчик навигации
	private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
	{
#if DEBUG
		// Проверяем, это консольное сообщение или обычная навигация
		if (e.Url.StartsWith("console://"))
		{
			// Это консольное сообщение - обрабатываем и отменяем навигацию
			e.Cancel = true;
			OnConsoleMessageReceived(e.Url);
			return;
		}
#endif
		// Обычная навигация - показываем индикатор загрузки
		loadingIndicator.IsVisible = true;
		loadingIndicator.IsRunning = true;
		errorLayout.IsVisible = false;
	}

	private async void OnNavigated(object? sender, WebNavigatedEventArgs e)
	{
		// Скрываем индикатор загрузки
		loadingIndicator.IsVisible = false;
		loadingIndicator.IsRunning = false;

		// Проверяем статус навигации
		if (e.Result != WebNavigationResult.Success)
		{
			ShowError($"Не удалось загрузить страницу. Статус: {e.Result}");
		}
		else
		{
#if DEBUG
			// Инжектируем скрипт после загрузки страницы (только в Debug)
			await Task.Delay(500); // Даём странице время для инициализации
			await InjectConsoleCapture();
#endif
			// Сохраняем куки после успешной навигации
			_ = SaveCookiesAsync();
		}
	}

	private void ShowError(string message)
	{
		errorLabel.Text = message;
		errorLayout.IsVisible = true;
		webView.IsVisible = false;
#if DEBUG
		AddConsoleMessage("error", $"[ERROR] {message}");
#endif
	}

	private void OnRefreshClicked(object? sender, EventArgs e)
	{
		errorLayout.IsVisible = false;
		webView.IsVisible = true;
		webView.Reload();
	}

#if DEBUG
	// ============================================
	// Методы консоли (доступны только в Debug)
	// ============================================

	private async Task InjectConsoleCapture()
	{
		var script = @"
(function() {
    // Проверяем, не был ли уже инициализирован перехват
    if (window.__consoleIntercepted) {
        return;
    }
    window.__consoleIntercepted = true;

    // Сохраняем оригинальные методы консоли
    const originalLog = console.log;
    const originalError = console.error;
    const originalWarn = console.warn;
    const originalInfo = console.info;

    // Функция для отправки сообщений в MAUI
    function sendToMaui(type, args) {
        try {
            const message = Array.from(args).map(arg => {
                if (typeof arg === 'object') {
                    try {
                        return JSON.stringify(arg, null, 2);
                    } catch(e) {
                        return String(arg);
                    }
                }
                return String(arg);
            }).join(' ');

            // Используем специальный протокол для передачи сообщений
            window.location = 'console://' + type + '/' + encodeURIComponent(message);
        } catch(e) {
            originalError('Failed to send to MAUI:', e);
        }
    }

    // Перехватываем console.log
    console.log = function() {
        originalLog.apply(console, arguments);
        sendToMaui('log', arguments);
    };

    // Перехватываем console.error
    console.error = function() {
        originalError.apply(console, arguments);
        sendToMaui('error', arguments);
    };

    // Перехватываем console.warn
    console.warn = function() {
        originalWarn.apply(console, arguments);
        sendToMaui('warn', arguments);
    };

    // Перехватываем console.info
    console.info = function() {
        originalInfo.apply(console, arguments);
        sendToMaui('info', arguments);
    };

    // Перехватываем глобальные ошибки
    window.addEventListener('error', function(event) {
        sendToMaui('error', [event.message + ' at ' + event.filename + ':' + event.lineno]);
    });

    // Перехватываем необработанные Promise ошибки
    window.addEventListener('unhandledrejection', function(event) {
        sendToMaui('error', ['Unhandled Promise Rejection: ' + event.reason]);
    });

    console.log('Console capture initialized');
})();
";

		try
		{
			await webView.EvaluateJavaScriptAsync(script);
		}
		catch (Exception ex)
		{
			AddConsoleMessage("error", $"[INIT ERROR] {ex.Message}");
		}
	}

	private void OnToggleConsoleClicked(object? sender, EventArgs e)
	{
		isConsoleVisible = !isConsoleVisible;

		if (isConsoleVisible)
		{
			// Открываем консоль
			consoleRow.Height = new GridLength(300);
			consolePanel.IsVisible = true;
			consoleToggleButton.IsVisible = false;
		}
		else
		{
			// Закрываем консоль
			consoleRow.Height = new GridLength(0);
			consolePanel.IsVisible = false;
			consoleToggleButton.IsVisible = true;
		}
	}

	private void OnClearConsoleClicked(object? sender, EventArgs e)
	{
		consoleContent.Clear();
		consoleMessages.Clear();
		lastMessage = "";
		messageCount = 0;
		lastMessageLabel = null;
		AddConsoleMessage("info", "Console cleared");
	}

	private async void OnCopyLogsClicked(object? sender, EventArgs e)
	{
		try
		{
			var logText = new StringBuilder();
			logText.AppendLine("=== WhiteBroker Console Logs ===");
			logText.AppendLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
			logText.AppendLine($"Всего сообщений: {consoleMessages.Count}");
			logText.AppendLine(new string('=', 40));
			logText.AppendLine();

			foreach (var msg in consoleMessages)
			{
				var countStr = msg.Count > 1 ? $" (x{msg.Count})" : "";
				logText.AppendLine($"[{msg.Timestamp:HH:mm:ss}] [{msg.Type.ToUpper()}]{countStr} {msg.Message}");
			}

			await Clipboard.SetTextAsync(logText.ToString());
			
			// Показываем уведомление
			await DisplayAlert("Готово", $"Логи скопированы в буфер обмена ({consoleMessages.Count} сообщений)", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ошибка", $"Не удалось скопировать логи: {ex.Message}", "OK");
		}
	}

	private void AddConsoleMessage(string type, string message)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			// Проверяем на дублирование
			if (message == lastMessage && lastMessageLabel != null)
			{
				// Увеличиваем счётчик
				messageCount++;
				var timestamp = consoleMessages[consoleMessages.Count - 1].Timestamp.ToString("HH:mm:ss");
				lastMessageLabel.Text = $"[{timestamp}] [{type.ToUpper()}] (x{messageCount}) {message}";
				
				// Обновляем запись в списке
				consoleMessages[consoleMessages.Count - 1].Count = messageCount;
			}
			else
			{
				// Новое сообщение
				lastMessage = message;
				messageCount = 1;

				var color = type switch
				{
					"error" => Colors.Red,
					"warn" => Colors.Orange,
					"info" => Colors.LightBlue,
					_ => Colors.White
				};

				var timestamp = DateTime.Now;
				var label = new Label
				{
					Text = $"[{timestamp:HH:mm:ss}] [{type.ToUpper()}] {message}",
					TextColor = color,
					FontSize = 12,
					FontFamily = "Courier New",
					Margin = new Thickness(0, 2)
				};

				consoleContent.Add(label);
				lastMessageLabel = label;

				// Сохраняем в список
				consoleMessages.Add(new ConsoleMessage
				{
					Timestamp = timestamp,
					Type = type,
					Message = message,
					Count = 1
				});

				// Автоматическая прокрутка вниз
				Device.BeginInvokeOnMainThread(async () =>
				{
					await Task.Delay(100);
					await consoleScrollView.ScrollToAsync(0, consoleContent.Height, false);
				});
			}
		});
	}

	private void OnConsoleMessageReceived(string url)
	{
		try
		{
			var urlWithoutScheme = url.Substring("console://".Length);
			var parts = urlWithoutScheme.Split(new[] { '/' }, 2);

			if (parts.Length == 2)
			{
				var type = parts[0];
				var message = Uri.UnescapeDataString(parts[1]);
				AddConsoleMessage(type, message);
			}
		}
		catch (Exception ex)
		{
			AddConsoleMessage("error", $"[PARSE ERROR] {ex.Message}");
		}
	}

	// Класс для хранения сообщений консоли
	private class ConsoleMessage
	{
		public DateTime Timestamp { get; set; }
		public string Type { get; set; } = "";
		public string Message { get; set; } = "";
		public int Count { get; set; } = 1;
	}
#else
	// ============================================
	// Заглушки для Release режима
	// ============================================
	
	// Эти методы нужны, чтобы XAML компилировался в Release
	// но они никогда не будут вызваны, так как кнопки скрыты
	
	private void OnToggleConsoleClicked(object? sender, EventArgs e)
	{
		// Заглушка - консоль недоступна в Release
	}

	private void OnClearConsoleClicked(object? sender, EventArgs e)
	{
		// Заглушка - консоль недоступна в Release
	}

	private void OnCopyLogsClicked(object? sender, EventArgs e)
	{
		// Заглушка - консоль недоступна в Release
	}
#endif
}
