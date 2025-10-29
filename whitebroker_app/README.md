# WhiteBroker App

Flutter-приложение с WebView для загрузки сайта WhiteBroker.

## Особенности

- ✅ WebView с поддержкой HTTP (без SSL сертификата)
- ✅ Настроенный splash screen с брокерской тематикой
- ✅ Поддержка Android и iOS
- ✅ Индикатор загрузки страницы
- ✅ Кнопка обновления страницы

## URL сайта

Приложение загружает страницу: `http://89.104.65.16/login`

## Настройки для HTTP

### Android
В файле `android/app/src/main/AndroidManifest.xml` добавлено:
- `android:usesCleartextTraffic="true"` - разрешает HTTP соединения
- Разрешения `INTERNET` и `ACCESS_NETWORK_STATE`

### iOS
В файле `ios/Runner/Info.plist` добавлены настройки `NSAppTransportSecurity`:
- `NSAllowsArbitraryLoads` - разрешает загрузку с небезопасных источников
- Исключение для домена `89.104.65.16`

## Запуск приложения

### Требования
- Flutter SDK 3.8.1 или выше
- Android Studio / Xcode (для эмуляторов)
- Устройство Android / iOS или эмулятор

### Установка зависимостей
```bash
cd whitebroker_app
flutter pub get
```

### Запуск на Android
```bash
flutter run
```

### Запуск на iOS (только на macOS)
```bash
flutter run -d ios
```

### Сборка APK для Android
```bash
flutter build apk --release
```
APK файл будет в: `build/app/outputs/flutter-apk/app-release.apk`

### Сборка для iOS
```bash
flutter build ios --release
```

## Структура проекта

```
whitebroker_app/
├── lib/
│   └── main.dart              # Основной файл приложения
├── android/                   # Настройки Android
├── ios/                       # Настройки iOS
├── assets/                    # Ресурсы (иконки, изображения)
└── pubspec.yaml              # Зависимости и конфигурация
```

## Используемые пакеты

- `webview_flutter: ^4.4.2` - WebView компонент
- `flutter_native_splash: ^2.3.5` - Splash screen

## Цветовая схема

- Основной цвет splash screen: `#2563EB` (синий)
- Темный режим: `#1E40AF` (темно-синий)

## Возможные проблемы

### Сайт не загружается на Android
- Убедитесь, что в `AndroidManifest.xml` установлен `android:usesCleartextTraffic="true"`
- Проверьте наличие разрешения `INTERNET`

### Сайт не загружается на iOS
- Проверьте настройки `NSAppTransportSecurity` в `Info.plist`
- Убедитесь, что домен добавлен в исключения

### Splash screen не исчезает
- Splash screen автоматически убирается после загрузки первой страницы
- При ошибке загрузки splash screen также будет убран

## Контакты

Для вопросов и предложений свяжитесь с разработчиком.
