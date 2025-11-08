using System.Text.Json;

namespace WhiteBroker.Services;

/// <summary>
/// Менеджер куков для сохранения и восстановления состояния авторизации
/// </summary>
public class CookieManager
{
    private const string CookiesFileName = "cookies.json";
    private readonly string _cookiesFilePath;

    public CookieManager()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _cookiesFilePath = Path.Combine(appDataPath, CookiesFileName);
    }

    /// <summary>
    /// Сохраняет куки в файл
    /// </summary>
    public async Task SaveCookiesAsync(List<CookieData> cookies)
    {
        try
        {
            var json = JsonSerializer.Serialize(cookies, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_cookiesFilePath, json);

#if DEBUG
            Console.WriteLine($"[CookieManager] Сохранено {cookies.Count} куков в {_cookiesFilePath}");
#endif
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[CookieManager] Ошибка при сохранении куков: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Загружает куки из файла
    /// </summary>
    public async Task<List<CookieData>> LoadCookiesAsync()
    {
        try
        {
            if (!File.Exists(_cookiesFilePath))
            {
#if DEBUG
                Console.WriteLine("[CookieManager] Файл куков не найден, возвращаем пустой список");
#endif
                return new List<CookieData>();
            }

            var json = await File.ReadAllTextAsync(_cookiesFilePath);
            var cookies = JsonSerializer.Deserialize<List<CookieData>>(json) ?? new List<CookieData>();

            // Фильтруем просроченные куки
            var validCookies = cookies.Where(c => !c.IsExpired()).ToList();

#if DEBUG
            Console.WriteLine($"[CookieManager] Загружено {validCookies.Count} валидных куков из {cookies.Count}");
#endif

            return validCookies;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[CookieManager] Ошибка при загрузке куков: {ex.Message}");
#endif
            return new List<CookieData>();
        }
    }

    /// <summary>
    /// Очищает сохраненные куки
    /// </summary>
    public async Task ClearCookiesAsync()
    {
        try
        {
            if (File.Exists(_cookiesFilePath))
            {
                File.Delete(_cookiesFilePath);
#if DEBUG
                Console.WriteLine("[CookieManager] Куки очищены");
#endif
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[CookieManager] Ошибка при очистке куков: {ex.Message}");
#endif
        }

        await Task.CompletedTask;
    }
}

/// <summary>
/// Класс для хранения данных кука
/// </summary>
public class CookieData
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Path { get; set; } = "/";
    public long ExpiresUnixTime { get; set; } // Unix timestamp в секундах
    public bool Secure { get; set; }
    public bool HttpOnly { get; set; }

    /// <summary>
    /// Проверяет, истек ли срок действия кука
    /// </summary>
    public bool IsExpired()
    {
        if (ExpiresUnixTime == 0)
        {
            // Сессионный кук, никогда не истекает в нашем контексте
            return false;
        }

        var expiresDate = DateTimeOffset.FromUnixTimeSeconds(ExpiresUnixTime);
        return expiresDate < DateTimeOffset.Now;
    }

    /// <summary>
    /// Получает дату истечения срока действия
    /// </summary>
    public DateTimeOffset? GetExpiresDate()
    {
        if (ExpiresUnixTime == 0)
        {
            return null; // Сессионный кук
        }

        return DateTimeOffset.FromUnixTimeSeconds(ExpiresUnixTime);
    }

    /// <summary>
    /// Устанавливает дату истечения срока действия
    /// </summary>
    public void SetExpiresDate(DateTimeOffset? expiresDate)
    {
        if (expiresDate.HasValue)
        {
            ExpiresUnixTime = expiresDate.Value.ToUnixTimeSeconds();
        }
        else
        {
            ExpiresUnixTime = 0; // Сессионный кук
        }
    }
}

