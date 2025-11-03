# Руководство по сборке White Broker MAUI

Подробная инструкция по сборке релизных версий приложения для различных платформ.

## 📋 Содержание

1. [Android](#android)
2. [iOS](#ios)
3. [Windows](#windows)
4. [macOS](#macos)
5. [Подписание и публикация](#подписание-и-публикация)

---

## Android

### Требования
- .NET 8.0 SDK
- Android SDK (API 21-34)
- Java JDK 17+

### Сборка Debug APK

```bash
dotnet build -f net8.0-android -c Debug
```

**Результат:** `bin/Debug/net8.0-android/com.whitebroker.app-Signed.apk`

### Сборка Release APK

```bash
dotnet publish -f net8.0-android -c Release
```

**Результат:** `bin/Release/net8.0-android/publish/com.whitebroker.app-Signed.apk`

### Сборка Android App Bundle (AAB)

```bash
dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=aab
```

**Результат:** `bin/Release/net8.0-android/publish/com.whitebroker.app-Signed.aab`

**Примечание:** AAB файлы требуются для публикации в Google Play Store.

### Подписание APK/AAB

#### Создание keystore

```bash
keytool -genkeypair -v -keystore whitebroker.keystore -alias whitebroker -keyalg RSA -keysize 2048 -validity 10000
```

#### Настройка подписания в .csproj

Добавьте в `WhiteBroker.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release' and '$(TargetFramework)' == 'net8.0-android'">
  <AndroidKeyStore>True</AndroidKeyStore>
  <AndroidSigningKeyStore>путь/к/whitebroker.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>whitebroker</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>ваш_пароль</AndroidSigningKeyPass>
  <AndroidSigningStorePass>ваш_пароль</AndroidSigningStorePass>
</PropertyGroup>
```

#### Или через командную строку

```bash
dotnet publish -f net8.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=whitebroker.keystore \
  -p:AndroidSigningKeyAlias=whitebroker \
  -p:AndroidSigningKeyPass=ваш_пароль \
  -p:AndroidSigningStorePass=ваш_пароль
```

### Оптимизация размера APK

Добавьте в `.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
  <AndroidEnableProguard>true</AndroidEnableProguard>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```

### Установка на устройство

```bash
adb install bin/Release/net8.0-android/publish/com.whitebroker.app-Signed.apk
```

---

## iOS

### Требования
- macOS с Xcode 15+
- .NET 8.0 SDK
- Apple Developer аккаунт (для установки на устройства)

### Сборка для симулятора

```bash
dotnet build -f net8.0-ios -c Debug
```

### Сборка Release (для устройства)

```bash
dotnet publish -f net8.0-ios -c Release -p:ArchiveOnBuild=true
```

**Результат:** `.ipa` файл в `bin/Release/net8.0-ios/publish/`

### Настройка подписания

#### 1. В Visual Studio for Mac
1. Откройте проект
2. Перейдите в Project Options → iOS Bundle Signing
3. Выберите Identity и Provisioning Profile
4. Соберите проект

#### 2. Через командную строку

```bash
dotnet publish -f net8.0-ios -c Release \
  -p:CodesignKey="iPhone Distribution: Your Name" \
  -p:CodesignProvision="Your Provisioning Profile"
```

### Публикация в App Store

1. **Создайте архив:**
```bash
dotnet publish -f net8.0-ios -c Release -p:ArchiveOnBuild=true
```

2. **Загрузите в App Store Connect:**
- Используйте Xcode → Window → Organizer
- Выберите архив и нажмите "Distribute App"
- Или используйте `altool`:

```bash
xcrun altool --upload-app \
  --type ios \
  --file "path/to/app.ipa" \
  --username "your@email.com" \
  --password "app-specific-password"
```

---

## Windows

### Требования
- Windows 10/11 (версия 1809+)
- .NET 8.0 SDK
- Windows SDK 10.0.19041.0+

### Сборка неупакованного приложения

```bash
dotnet build -f net8.0-windows10.0.19041.0 -c Release
```

**Результат:** `bin/Release/net8.0-windows10.0.19041.0/win10-x64/`

### Сборка MSIX пакета

```bash
dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:GenerateAppxPackageOnBuild=true
```

**Результат:** `.msix` файл в `bin/Release/net8.0-windows10.0.19041.0/win10-x64/AppPackages/`

### Создание самоподписанного сертификата

```powershell
New-SelfSignedCertificate -Type Custom -Subject "CN=WhiteBroker" -KeyUsage DigitalSignature -FriendlyName "WhiteBroker Cert" -CertStoreLocation "Cert:\CurrentUser\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
```

### Подписание MSIX

```powershell
signtool sign /fd SHA256 /a /f MyCertificate.pfx /p YourPassword "path\to\app.msix"
```

### Sideloading на Windows

1. Установите сертификат в Trusted Root
2. Включите Developer Mode в Windows Settings
3. Двойной клик на `.msix` файл

---

## macOS

### Требования
- macOS с Xcode 15+
- .NET 8.0 SDK
- Apple Developer аккаунт (для распространения)

### Сборка для локального запуска

```bash
dotnet build -f net8.0-maccatalyst -c Debug
```

### Сборка Release

```bash
dotnet publish -f net8.0-maccatalyst -c Release
```

**Результат:** `.app` пакет в `bin/Release/net8.0-maccatalyst/maccatalyst-x64/publish/`

### Создание PKG инсталлятора

```bash
productbuild --component "WhiteBroker.app" /Applications "WhiteBroker.pkg"
```

### Подписание для распространения

```bash
codesign --force --deep --sign "Developer ID Application: Your Name" "WhiteBroker.app"
```

### Нотаризация (для распространения вне App Store)

```bash
xcrun notarytool submit WhiteBroker.pkg \
  --apple-id "your@email.com" \
  --team-id "YOUR_TEAM_ID" \
  --password "app-specific-password" \
  --wait
```

---

## Подписание и публикация

### Google Play Store (Android)

#### Подготовка
1. Создайте подписанный AAB файл
2. Зарегистрируйтесь в [Google Play Console](https://play.google.com/console)
3. Создайте новое приложение

#### Загрузка
1. В Play Console выберите "Production" → "Create new release"
2. Загрузите AAB файл
3. Заполните информацию о релизе
4. Отправьте на ревью

#### Требования
- Иконка приложения (512x512)
- Feature graphic (1024x500)
- Скриншоты (минимум 2)
- Описание приложения
- Политика конфиденциальности

### Apple App Store (iOS)

#### Подготовка
1. Зарегистрируйтесь в [App Store Connect](https://appstoreconnect.apple.com)
2. Создайте новое приложение
3. Подготовьте метаданные и скриншоты

#### Загрузка
1. Создайте `.ipa` файл с релизной конфигурацией
2. Загрузите через Xcode Organizer или `altool`
3. Заполните информацию в App Store Connect
4. Отправьте на ревью

#### Требования
- Иконка (1024x1024)
- Скриншоты для разных размеров экрана
- Описание и ключевые слова
- Политика конфиденциальности

### Microsoft Store (Windows)

#### Подготовка
1. Зарегистрируйтесь в [Partner Center](https://partner.microsoft.com)
2. Зарезервируйте имя приложения

#### Загрузка
1. Создайте MSIX пакет
2. Загрузите через Partner Center
3. Заполните информацию о приложении
4. Отправьте на сертификацию

### Mac App Store (macOS)

Аналогично iOS App Store, но с использованием macOS специфичных настроек и скриншотов.

---

## Автоматизация CI/CD

### GitHub Actions (пример)

```yaml
name: Build Android

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Install MAUI
      run: dotnet workload install maui
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build Android
      run: dotnet publish -f net8.0-android -c Release
    
    - name: Upload APK
      uses: actions/upload-artifact@v3
      with:
        name: android-apk
        path: bin/Release/net8.0-android/publish/*.apk
```

---

## Полезные команды

### Проверка версии
```bash
dotnet --version
dotnet workload list
```

### Очистка проекта
```bash
dotnet clean
rm -rf bin/ obj/
```

### Информация о сборке
```bash
dotnet build -v detailed
```

### Размер приложения
```bash
# Android APK
ls -lh bin/Release/net8.0-android/publish/*.apk

# iOS IPA
ls -lh bin/Release/net8.0-ios/publish/*.ipa
```

---

## Решение проблем

### "Certificate not found"
- Убедитесь, что сертификат установлен в системе
- Проверьте правильность имени сертификата

### "Provisioning profile doesn't match"
- Обновите Provisioning Profile в Apple Developer Portal
- Синхронизируйте профили в Xcode

### Большой размер приложения
- Включите `PublishTrimmed`
- Используйте AOT компиляцию (Android)
- Проверьте включенные ресурсы

### Ошибки подписания Android
- Проверьте пароль keystore
- Убедитесь, что keystore файл доступен

---

## Дополнительные ресурсы

- [Публикация Android приложений](https://learn.microsoft.com/dotnet/maui/android/deployment/)
- [Публикация iOS приложений](https://learn.microsoft.com/dotnet/maui/ios/deployment/)
- [Windows MSIX packaging](https://learn.microsoft.com/windows/msix/)
- [Google Play Console](https://play.google.com/console)
- [App Store Connect](https://appstoreconnect.apple.com)

---

**Готово!** Теперь вы можете собирать и публиковать White Broker MAUI на всех платформах! 🚀







