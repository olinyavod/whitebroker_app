# Руководство по версионированию WhiteBroker

## 📋 Где указывается версия

Версия приложения указывается в файле **`whitebroker_maui/WhiteBroker.csproj`**:

```xml
<ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

## 🔢 Два типа версий

### 1. ApplicationDisplayVersion (Отображаемая версия)

**Формат**: `Major.Minor.Patch` (например: `1.0.0`, `1.2.3`, `2.0.0`)

**Назначение**: 
- Это версия, которую видят пользователи
- Отображается в магазинах приложений (Google Play, App Store)
- Показывается в настройках приложения

**Семантическое версионирование**:
- **Major** (1.x.x) - крупные изменения, несовместимые с предыдущими версиями
- **Minor** (x.1.x) - новые функции, обратно совместимые
- **Patch** (x.x.1) - исправления багов

### 2. ApplicationVersion (Номер сборки)

**Формат**: Целое число (например: `1`, `2`, `100`)

**Назначение**:
- Уникальный идентификатор каждой сборки
- Используется магазинами для определения новизны версии
- **ДОЛЖЕН всегда увеличиваться** с каждым релизом

**Важно**: 
- Google Play и App Store **требуют**, чтобы каждая новая загрузка имела больший ApplicationVersion
- Нельзя загрузить версию с меньшим или равным номером сборки

## 🚀 Как создать новый релиз

### Шаг 1: Обновите версии в WhiteBroker.csproj

```xml
<!-- Было -->
<ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>

<!-- Стало -->
<ApplicationDisplayVersion>1.1.0</ApplicationDisplayVersion>
<ApplicationVersion>2</ApplicationVersion>
```

### Шаг 2: Закоммитьте изменения с префиксом #Release

```bash
git add whitebroker_maui/WhiteBroker.csproj
git commit -m "#Release v1.1.0 - Добавлена новая функция X"
git push origin main
```

### Шаг 3: GitHub Actions автоматически

1. ✅ Прочитает версию из `WhiteBroker.csproj`
2. ✅ Соберет APK и AAB с указанной версией
3. ✅ Создаст GitHub Release с тегом `v1.1.0`
4. ✅ Загрузит файлы: `WhiteBroker-v1.1.0-Release.apk` и `.aab`
5. ✅ Отправит уведомление в Telegram (если настроено)

## 📝 Примеры версионирования

### Исправление бага
```xml
<!-- Было: 1.0.0 (build 1) -->
<ApplicationDisplayVersion>1.0.1</ApplicationDisplayVersion>
<ApplicationVersion>2</ApplicationVersion>
```
```bash
git commit -m "#Release v1.0.1 - Исправлена ошибка входа"
```

### Новая функция
```xml
<!-- Было: 1.0.1 (build 2) -->
<ApplicationDisplayVersion>1.1.0</ApplicationDisplayVersion>
<ApplicationVersion>3</ApplicationVersion>
```
```bash
git commit -m "#Release v1.1.0 - Добавлена темная тема"
```

### Крупное обновление
```xml
<!-- Было: 1.5.3 (build 15) -->
<ApplicationDisplayVersion>2.0.0</ApplicationDisplayVersion>
<ApplicationVersion>16</ApplicationVersion>
```
```bash
git commit -m "#Release v2.0.0 - Новый дизайн и архитектура"
```

## 🔍 Где используется версия

### В Android APK/AAB:
- **versionName** = `ApplicationDisplayVersion` (например: "1.0.0")
- **versionCode** = `ApplicationVersion` (например: 1)

### В GitHub Release:
- **Tag** = `v{ApplicationDisplayVersion}` (например: `v1.0.0`)
- **Title** = `WhiteBroker v{ApplicationDisplayVersion}` (например: "WhiteBroker v1.0.0")
- **Files** = `WhiteBroker-v{ApplicationDisplayVersion}-Release.apk`

### В Telegram уведомлении:
```
✅ WhiteBroker v1.0.0 успешно собран!

📱 Собранные файлы:
• Release APK (подписанный) - для распространения
• Release AAB (подписанный) - для публикации в Google Play

🔗 Релиз: https://github.com/user/repo/releases/tag/v1.0.0
```

## ⚠️ Важные правила

### ✅ ОБЯЗАТЕЛЬНО:
1. **Всегда увеличивайте ApplicationVersion** перед новым релизом
2. **Используйте семантическое версионирование** для ApplicationDisplayVersion
3. **Коммитьте с префиксом #Release** для запуска сборки

### ❌ НЕЛЬЗЯ:
1. Уменьшать ApplicationVersion (магазины не примут)
2. Использовать одинаковый ApplicationVersion для разных сборок
3. Забывать обновлять версию перед релизом

## 🛠️ Автоматизация (опционально)

Если хотите автоматизировать увеличение версии, можно создать скрипт:

```powershell
# bump-version.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$Type  # major, minor, patch
)

$csprojPath = "whitebroker_maui/WhiteBroker.csproj"
$content = Get-Content $csprojPath -Raw

# Читаем текущую версию
$versionMatch = [regex]::Match($content, '<ApplicationDisplayVersion>(\d+)\.(\d+)\.(\d+)</ApplicationDisplayVersion>')
$major = [int]$versionMatch.Groups[1].Value
$minor = [int]$versionMatch.Groups[2].Value
$patch = [int]$versionMatch.Groups[3].Value

# Читаем build number
$buildMatch = [regex]::Match($content, '<ApplicationVersion>(\d+)</ApplicationVersion>')
$build = [int]$buildMatch.Groups[1].Value

# Увеличиваем версию
switch ($Type) {
    "major" { $major++; $minor = 0; $patch = 0 }
    "minor" { $minor++; $patch = 0 }
    "patch" { $patch++ }
}
$build++

$newVersion = "$major.$minor.$patch"

# Обновляем файл
$content = $content -replace '<ApplicationDisplayVersion>\d+\.\d+\.\d+</ApplicationDisplayVersion>', "<ApplicationDisplayVersion>$newVersion</ApplicationDisplayVersion>"
$content = $content -replace '<ApplicationVersion>\d+</ApplicationVersion>', "<ApplicationVersion>$build</ApplicationVersion>"
[System.IO.File]::WriteAllText($csprojPath, $content)

Write-Host "✅ Version updated: $newVersion (build $build)"
Write-Host "Next steps:"
Write-Host "1. git add $csprojPath"
Write-Host "2. git commit -m `"#Release v$newVersion - Your changes description`""
Write-Host "3. git push origin main"
```

**Использование**:
```powershell
# Patch (1.0.0 → 1.0.1)
./bump-version.ps1 -Type patch

# Minor (1.0.1 → 1.1.0)
./bump-version.ps1 -Type minor

# Major (1.1.0 → 2.0.0)
./bump-version.ps1 -Type major
```

## 📚 Дополнительная информация

- [Семантическое версионирование](https://semver.org/lang/ru/)
- [Android versioning](https://developer.android.com/studio/publish/versioning)
- [.NET MAUI App versioning](https://learn.microsoft.com/dotnet/maui/deployment/)

