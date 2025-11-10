# GitHub Actions Workflows

## Build and Release WhiteBroker

### Описание
Автоматическая сборка и релиз .NET MAUI Android приложения WhiteBroker.

### Условия запуска

Workflow запускается в двух случаях:

1. **Автоматически при коммите в ветку `main`**, если сообщение коммита начинается с `#Release`:
   ```bash
   git commit -m "#Release Добавлена новая функция"
   git push origin main
   ```

2. **Вручную через GitHub UI** (workflow_dispatch):
   - Перейдите в Actions → Build and Release WhiteBroker → Run workflow
   - Укажите версию релиза вручную

### Что собирается

- ✅ **Release APK** (подписанный) - для распространения и установки на устройства
- ✅ **Release AAB** (подписанный) - для публикации в Google Play Store

### Версионирование

Версия автоматически читается из файла `whitebroker_maui/WhiteBroker.csproj`:
```xml
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
```

При ручном запуске через workflow_dispatch можно указать версию вручную.

### Требуемые настройки

#### Secrets (необязательно, для Telegram уведомлений)
- `TELEGRAM_BOT_TOKEN` - токен Telegram бота

#### Variables (необязательно, для Telegram уведомлений)
- `TELEGRAM_CHAT_ID` - ID чата для уведомлений

### Примеры использования

#### Создание нового релиза
```bash
# 1. Обновите версию в WhiteBroker.csproj
# 2. Закоммитьте изменения с префиксом #Release
git add whitebroker_maui/WhiteBroker.csproj
git commit -m "#Release v1.1 - Исправлены баги и добавлены новые функции"
git push origin main
```

#### Обычный коммит (без релиза)
```bash
# Workflow НЕ запустится, если сообщение не начинается с #Release
git commit -m "Исправлена опечатка в документации"
git push origin main
```

### Результат

После успешной сборки:
- Создается GitHub Release с тегом `v{VERSION}`
- К релизу прикрепляются APK и AAB файлы
- Отправляется Telegram уведомление (если настроено)

### Особенности

- ❌ **НЕ использует** Actions Storage (upload-artifact/download-artifact)
- ✅ Артефакты загружаются напрямую в GitHub Release
- ✅ Собирается только MAUI проект (Flutter и Ionic не затрагиваются)
- ✅ Экономия времени и ресурсов - собирается только Release сборка

