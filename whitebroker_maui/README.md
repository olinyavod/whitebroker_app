# White Broker MAUI App

Кроссплатформенное приложение на .NET MAUI, которое открывает веб-интерфейс White Broker в WebView.

## Описание

Это приложение использует .NET MAUI (Multi-platform App UI) для создания нативного приложения для Android, iOS, Windows и macOS. Приложение открывает URL `http://89.104.65.16/login` в встроенном WebView компоненте.

## Возможности

- ✅ Открытие веб-сайта в нативном WebView
- ✅ Полноэкранный режим
- ✅ Индикатор загрузки страницы
- ✅ Обработка ошибок загрузки
- ✅ Поддержка работы с камерой и файлами
- ✅ HTTP-трафик (cleartext traffic) разрешен для работы с незащищенными соединениями

## Поддерживаемые платформы

- **Android** (API 21+)
- **iOS** (iOS 13.0+)
- **Windows** (Windows 10 версия 17763+)
- **macOS** (macOS 10.13+)

## Требования

### Для разработки

- **.NET 8.0 SDK** или новее
- **Visual Studio 2022** (версия 17.8+) или **Visual Studio Code** с расширением C# Dev Kit
- Для Android: Android SDK (API 21-34)
- Для iOS/macOS: Xcode 15+ (только на macOS)

### Установка .NET MAUI Workload

```bash
dotnet workload install maui
```

## Сборка и запуск

### Android

```bash
# Сборка
dotnet build -f net8.0-android

# Запуск на эмуляторе или устройстве
dotnet run -f net8.0-android
```

Или через Visual Studio:
1. Откройте `WhiteBroker.csproj`
2. Выберите целевую платформу "Android"
3. Выберите устройство или эмулятор
4. Нажмите F5 для запуска

### iOS

```bash
# Сборка (только на macOS)
dotnet build -f net8.0-ios

# Запуск на симуляторе
dotnet run -f net8.0-ios
```

### Windows

```bash
# Сборка
dotnet build -f net8.0-windows10.0.19041.0

# Запуск
dotnet run -f net8.0-windows10.0.19041.0
```

### macOS

```bash
# Сборка (только на macOS)
dotnet build -f net8.0-maccatalyst

# Запуск
dotnet run -f net8.0-maccatalyst
```

## Структура проекта

```
whitebroker_maui/
├── App.xaml                          # Главный файл приложения
├── App.xaml.cs                       # Код-behind для App
├── MainPage.xaml                     # Главная страница с WebView
├── MainPage.xaml.cs                  # Логика главной страницы
├── MauiProgram.cs                    # Точка входа и конфигурация
├── WhiteBroker.csproj               # Файл проекта
├── Platforms/                        # Платформо-специфичный код
│   ├── Android/
│   │   ├── AndroidManifest.xml      # Разрешения для Android
│   │   ├── MainActivity.cs
│   │   └── MainApplication.cs
│   ├── iOS/
│   │   ├── AppDelegate.cs
│   │   ├── Info.plist               # Настройки и разрешения для iOS
│   │   └── Program.cs
│   └── Windows/
│       ├── App.xaml
│       ├── App.xaml.cs
│       └── Package.appxmanifest     # Манифест для Windows
└── Resources/                        # Ресурсы приложения
    ├── AppIcon/                      # Иконка приложения
    ├── Splash/                       # Экран-заставка
    ├── Images/                       # Изображения
    ├── Fonts/                        # Шрифты
    ├── Raw/                          # Сырые ресурсы
    └── Styles/                       # XAML стили
        ├── Colors.xaml
        └── Styles.xaml
```

## Настройки

### Изменить URL

Чтобы изменить открываемый URL, отредактируйте файл `MainPage.xaml`:

```xml
<WebView x:Name="webView"
         Source="http://89.104.65.16/login"  <!-- Измените здесь -->
         ... />
```

### Разрешения

#### Android
Разрешения настраиваются в `Platforms/Android/AndroidManifest.xml`:
- Интернет
- Камера
- Чтение/запись файлов

#### iOS
Разрешения настраиваются в `Platforms/iOS/Info.plist`:
- Камера
- Фотогалерея
- Небезопасные HTTP-соединения (NSAppTransportSecurity)

## Отличия от Flutter версии

- **Технология**: .NET MAUI вместо Flutter
- **Язык**: C# вместо Dart
- **UI**: XAML для разметки интерфейса
- **Нативные возможности**: Прямой доступ к платформенным API через условную компиляция
- **Размер приложения**: Обычно меньше, чем Flutter (нет движка Flutter)

## Известные проблемы

1. **HTTP вместо HTTPS**: Приложение использует незащищенное HTTP-соединение. В AndroidManifest.xml включен `usesCleartextTraffic="true"`, а в iOS Info.plist добавлен `NSAllowsArbitraryLoads`.

2. **WebView на iOS**: Для работы с HTTP-сайтами на iOS требуется настройка App Transport Security.

## Решение проблем

### Ошибка "workload not installed"
```bash
dotnet workload install maui
```

### Ошибка при сборке Android
```bash
# Очистка кэша
dotnet clean
dotnet build-server shutdown

# Пересборка
dotnet build -f net8.0-android
```

### Проблемы с WebView на Android
Убедитесь, что в `AndroidManifest.xml` указано `usesCleartextTraffic="true"` для работы с HTTP.

## Лицензия

Этот проект является частью White Broker и использует ту же лицензию, что и основной проект.

## Дополнительная информация

- [Документация .NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [WebView в MAUI](https://learn.microsoft.com/dotnet/maui/user-interface/controls/webview)
- [Платформенный код в MAUI](https://learn.microsoft.com/dotnet/maui/platform-integration/)








