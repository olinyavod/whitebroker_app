# Скрипт для увеличения версии WhiteBroker
# Использование:
#   ./bump-version.ps1 -Type patch  # 1.0.0 → 1.0.1
#   ./bump-version.ps1 -Type minor  # 1.0.1 → 1.1.0
#   ./bump-version.ps1 -Type major  # 1.1.0 → 2.0.0

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch")]
    [string]$Type,
    
    [Parameter(Mandatory=$false)]
    [string]$Message = ""
)

$ErrorActionPreference = "Stop"

# Путь к csproj файлу
$csprojPath = "whitebroker_maui/WhiteBroker.csproj"

if (-not (Test-Path $csprojPath)) {
    Write-Error "❌ Файл не найден: $csprojPath"
    Write-Host "Убедитесь, что вы запускаете скрипт из корня репозитория."
    exit 1
}

# Читаем файл
$content = Get-Content $csprojPath -Raw

# Извлекаем текущую версию
$versionMatch = [regex]::Match($content, '<ApplicationDisplayVersion>(\d+)\.(\d+)\.(\d+)</ApplicationDisplayVersion>')
if (-not $versionMatch.Success) {
    Write-Error "❌ Не удалось найти ApplicationDisplayVersion в $csprojPath"
    exit 1
}

$major = [int]$versionMatch.Groups[1].Value
$minor = [int]$versionMatch.Groups[2].Value
$patch = [int]$versionMatch.Groups[3].Value
$oldVersion = "$major.$minor.$patch"

# Извлекаем build number
$buildMatch = [regex]::Match($content, '<ApplicationVersion>(\d+)</ApplicationVersion>')
if (-not $buildMatch.Success) {
    Write-Error "❌ Не удалось найти ApplicationVersion в $csprojPath"
    exit 1
}

$build = [int]$buildMatch.Groups[1].Value
$oldBuild = $build

# Увеличиваем версию
Write-Host "`n🔄 Обновление версии..." -ForegroundColor Cyan
Write-Host "Текущая версия: $oldVersion (build $oldBuild)" -ForegroundColor Gray

switch ($Type) {
    "major" { 
        $major++
        $minor = 0
        $patch = 0
        Write-Host "Тип обновления: MAJOR (крупные изменения)" -ForegroundColor Yellow
    }
    "minor" { 
        $minor++
        $patch = 0
        Write-Host "Тип обновления: MINOR (новые функции)" -ForegroundColor Yellow
    }
    "patch" { 
        $patch++
        Write-Host "Тип обновления: PATCH (исправления)" -ForegroundColor Yellow
    }
}
$build++

$newVersion = "$major.$minor.$patch"

# Обновляем файл
$content = $content -replace '<ApplicationDisplayVersion>\d+\.\d+\.\d+</ApplicationDisplayVersion>', "<ApplicationDisplayVersion>$newVersion</ApplicationDisplayVersion>"
$content = $content -replace '<ApplicationVersion>\d+</ApplicationVersion>', "<ApplicationVersion>$build</ApplicationVersion>"

# Сохраняем с правильной кодировкой (UTF-8 с BOM для совместимости с Visual Studio)
$utf8BOM = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($csprojPath, $content, $utf8BOM)

Write-Host "`n✅ Версия обновлена успешно!" -ForegroundColor Green
Write-Host "Новая версия: $newVersion (build $build)" -ForegroundColor Green

# Показываем изменения
Write-Host "`n📝 Изменения:" -ForegroundColor Cyan
Write-Host "  ApplicationDisplayVersion: $oldVersion → $newVersion"
Write-Host "  ApplicationVersion: $oldBuild → $build"

# Следующие шаги
Write-Host "`n📋 Следующие шаги:" -ForegroundColor Cyan
Write-Host "1. Проверьте изменения:" -ForegroundColor White
Write-Host "   git diff $csprojPath" -ForegroundColor Gray

Write-Host "`n2. Закоммитьте и запустите релиз:" -ForegroundColor White
Write-Host "   git add $csprojPath" -ForegroundColor Gray

if ($Message -ne "") {
    $commitMsg = "#Release v$newVersion - $Message"
    Write-Host "   git commit -m `"$commitMsg`"" -ForegroundColor Gray
} else {
    Write-Host "   git commit -m `"#Release v$newVersion - [Опишите изменения]`"" -ForegroundColor Gray
}

Write-Host "   git push origin main" -ForegroundColor Gray

Write-Host "`n3. GitHub Actions автоматически:" -ForegroundColor White
Write-Host "   • Соберет APK и AAB" -ForegroundColor Gray
Write-Host "   • Создаст Release с тегом v$newVersion" -ForegroundColor Gray
Write-Host "   • Загрузит WhiteBroker-v$newVersion-Release.apk" -ForegroundColor Gray

# Предлагаем автоматически добавить в git
Write-Host "`n❓ Добавить изменения в git? (y/n): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq "y" -or $response -eq "Y" -or $response -eq "yes") {
    git add $csprojPath
    Write-Host "✅ Файл добавлен в git" -ForegroundColor Green
    
    if ($Message -ne "") {
        $commitMsg = "#Release v$newVersion - $Message"
        Write-Host "`n❓ Создать коммит с сообщением: `"$commitMsg`"? (y/n): " -ForegroundColor Yellow -NoNewline
        $commitResponse = Read-Host
        
        if ($commitResponse -eq "y" -or $commitResponse -eq "Y" -or $commitResponse -eq "yes") {
            git commit -m $commitMsg
            Write-Host "✅ Коммит создан" -ForegroundColor Green
            Write-Host "`nТеперь выполните: git push origin main" -ForegroundColor Cyan
        }
    } else {
        Write-Host "`nСоздайте коммит с описанием изменений:" -ForegroundColor Cyan
        Write-Host "git commit -m `"#Release v$newVersion - [Ваше описание]`"" -ForegroundColor Gray
    }
}

Write-Host ""

