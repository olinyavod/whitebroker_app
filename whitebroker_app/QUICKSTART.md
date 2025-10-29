# WhiteBroker App - Быстрый старт

## Описание
Flutter приложение с WebView для загрузки сайта http://89.104.65.16/login

## ⚡ Быстрый запуск

### 1. Установите зависимости
```bash
cd whitebroker_app
flutter pub get
```

### 2. Запустите приложение
```bash
flutter run
```

### 3. Соберите APK (Android)
```bash
flutter build apk --release
```
**APK находится:** `build/app/outputs/flutter-apk/app-release.apk`

## ✅ Что уже настроено

- ✅ WebView с HTTP поддержкой (без SSL)
- ✅ Splash screen (синий фон)
- ✅ Android манифест с `usesCleartextTraffic="true"`
- ✅ iOS Info.plist с `NSAppTransportSecurity`
- ✅ Индикатор загрузки
- ✅ Кнопка обновления страницы

## 📱 Скриншоты работы

При запуске:
1. Показывается синий splash screen
2. Загружается страница http://89.104.65.16/login
3. Splash screen исчезает после загрузки
4. Отображается WebView с сайтом

## 🛠 Технические детали

**URL:** http://89.104.65.16/login  
**Цвет splash:** #2563EB (синий)  
**Пакеты:**
- webview_flutter: ^4.4.2
- flutter_native_splash: ^2.3.5

## 🚨 Важные файлы

- `lib/main.dart` - основной код
- `android/app/src/main/AndroidManifest.xml` - настройки Android
- `ios/Runner/Info.plist` - настройки iOS
- `pubspec.yaml` - зависимости

## 💡 Полезные команды

```bash
# Анализ кода
flutter analyze

# Запуск на конкретном устройстве
flutter devices
flutter run -d <device_id>

# Сборка для разных платформ
flutter build apk --release          # Android APK
flutter build appbundle --release    # Android Bundle
flutter build ios --release          # iOS (только на macOS)
```

## 📄 Подробная документация
Смотрите [README.md](README.md) для полной документации.

