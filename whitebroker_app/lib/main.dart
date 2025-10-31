import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:webview_flutter/webview_flutter.dart';
import 'package:webview_flutter_android/webview_flutter_android.dart';
import 'package:permission_handler/permission_handler.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Полноэкранный режим
  SystemChrome.setEnabledSystemUIMode(
    SystemUiMode.immersiveSticky,
  );
  
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'White Broker',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.blue,
          brightness: Brightness.light,
        ),
        useMaterial3: true,
      ),
      darkTheme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.blue,
          brightness: Brightness.dark,
        ),
        useMaterial3: true,
      ),
      themeMode: ThemeMode.system,
      debugShowCheckedModeBanner: false,
      home: const WebViewPage(),
    );
  }
}

class WebViewPage extends StatefulWidget {
  const WebViewPage({super.key});

  @override
  State<WebViewPage> createState() => _WebViewPageState();
}

class _WebViewPageState extends State<WebViewPage> {
  late final WebViewController _controller;
  String? _errorMessage;
  bool _isLoading = true;
  bool _isFirstLoad = true;
  final List<String> _consoleLogs = []; // Для хранения логов JavaScript
  bool _showConsole = false; // Показывать ли консоль

  @override
  void initState() {
    super.initState();
    _requestPermissions();
    _initWebView();
  }

  // Запрашиваем разрешения для работы с файлами
  Future<void> _requestPermissions() async {
    // Для Android 13+ нужны разные разрешения
    if (await Permission.photos.isDenied) {
      await Permission.photos.request();
    }
    if (await Permission.videos.isDenied) {
      await Permission.videos.request();
    }
    if (await Permission.audio.isDenied) {
      await Permission.audio.request();
    }
    // Для камеры
    if (await Permission.camera.isDenied) {
      await Permission.camera.request();
    }
    // Для старых версий Android
    if (await Permission.storage.isDenied) {
      await Permission.storage.request();
    }
  }

  void _initWebView() {
    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..addJavaScriptChannel(
        'ConsoleLogger',
        onMessageReceived: (JavaScriptMessage message) {
          setState(() {
            _consoleLogs.add('[JS] ${message.message}');
            debugPrint('JavaScript Console: ${message.message}');
          });
        },
      )
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageStarted: (String url) {
            // Сбрасываем ошибку при начале новой загрузки
            setState(() {
              _errorMessage = null;
              _isLoading = true;
            });
            
            // Внедряем скрипт для перехвата console сразу при старте
            _controller.runJavaScript('''
              (function() {
                // Проверяем, не был ли уже установлен перехват
                if (window._consoleLoggerInstalled) {
                  console.log('Console logger уже установлен');
                  return;
                }
                window._consoleLoggerInstalled = true;
                
                // Перехватываем console.log
                const originalLog = console.log;
                console.log = function(...args) {
                  try {
                    ConsoleLogger.postMessage('LOG: ' + args.join(' '));
                  } catch(e) {}
                  originalLog.apply(console, args);
                };
                
                // Перехватываем console.error
                const originalError = console.error;
                console.error = function(...args) {
                  try {
                    ConsoleLogger.postMessage('ERROR: ' + args.join(' '));
                  } catch(e) {}
                  originalError.apply(console, args);
                };
                
                // Перехватываем console.warn
                const originalWarn = console.warn;
                console.warn = function(...args) {
                  try {
                    ConsoleLogger.postMessage('WARN: ' + args.join(' '));
                  } catch(e) {}
                  originalWarn.apply(console, args);
                };
                
                // Перехватываем необработанные ошибки
                window.addEventListener('error', function(e) {
                  try {
                    ConsoleLogger.postMessage('UNCAUGHT ERROR: ' + e.message + ' at ' + e.filename + ':' + e.lineno);
                  } catch(err) {}
                });
                
                // Перехватываем отклоненные промисы
                window.addEventListener('unhandledrejection', function(e) {
                  try {
                    ConsoleLogger.postMessage('UNHANDLED PROMISE REJECTION: ' + e.reason);
                  } catch(err) {}
                });
                
                // Сообщение о том, что логирование включено
                console.log('🔍 JavaScript Console Logging активирован (onPageStarted)');
              })();
            ''');
          },
          onPageFinished: (String url) {
            setState(() {
              _isLoading = false;
            });
            
            // Убираем флаг первой загрузки
            if (_isFirstLoad) {
              _isFirstLoad = false;
            }
            
            // Дополнительная проверка JS и теперь тестовый лог
            _controller.runJavaScript('''
              (function() {
                // Проверяем, установлен ли уже логгер
                if (!window._consoleLoggerInstalled) {
                  console.error('⚠️ Console logger НЕ БЫЛ установлен в onPageStarted, устанавливаем сейчас');
                  window._consoleLoggerInstalled = true;
                  
                  // Перехватываем console.log
                  const originalLog = console.log;
                  console.log = function(...args) {
                    try {
                      ConsoleLogger.postMessage('LOG: ' + args.join(' '));
                    } catch(e) {}
                    originalLog.apply(console, args);
                  };
                  
                  // Перехватываем console.error
                  const originalError = console.error;
                  console.error = function(...args) {
                    try {
                      ConsoleLogger.postMessage('ERROR: ' + args.join(' '));
                    } catch(e) {}
                    originalError.apply(console, args);
                  };
                  
                  // Перехватываем console.warn
                  const originalWarn = console.warn;
                  console.warn = function(...args) {
                    try {
                      ConsoleLogger.postMessage('WARN: ' + args.join(' '));
                    } catch(e) {}
                    originalWarn.apply(console, args);
                  };
                  
                  // Перехватываем необработанные ошибки
                  window.addEventListener('error', function(e) {
                    try {
                      ConsoleLogger.postMessage('UNCAUGHT ERROR: ' + e.message + ' at ' + e.filename + ':' + e.lineno);
                    } catch(err) {}
                  });
                  
                  // Перехватываем отклоненные промисы
                  window.addEventListener('unhandledrejection', function(e) {
                    try {
                      ConsoleLogger.postMessage('UNHANDLED PROMISE REJECTION: ' + e.reason);
                    } catch(err) {}
                  });
                }
                
                // Тестовое сообщение
                console.log('✅ Страница полностью загружена, JavaScript работает');
                console.log('📋 URL: ' + window.location.href);
                
                // Проверяем, есть ли кнопки загрузки на странице
                const buttons = document.querySelectorAll('button, input[type="button"], input[type="file"], [role="button"]');
                console.log('🔘 Найдено кнопок/элементов: ' + buttons.length);
                
                // Проверяем input[type="file"]
                const fileInputs = document.querySelectorAll('input[type="file"]');
                console.log('📎 Найдено input[type="file"]: ' + fileInputs.length);
                
                if (fileInputs.length > 0) {
                  fileInputs.forEach((input, idx) => {
                    console.log('  Input #' + idx + ': id=' + input.id + ', name=' + input.name + ', accept=' + input.accept);
                  });
                }
              })();
            ''');
          },
          onWebResourceError: (WebResourceError error) {
            debugPrint('WebView error: ${error.description}');
            
            setState(() {
              _isLoading = false;
            });
            
            // Убираем флаг первой загрузки даже при ошибке
            if (_isFirstLoad) {
              _isFirstLoad = false;
            }
            
            // Устанавливаем ошибку
            setState(() {
              _errorMessage = 'Ошибка загрузки: ${error.description}';
            });
          },
        ),
      )
      ..loadRequest(Uri.parse('http://89.104.65.16/login'));
    
    // Включаем поддержку выбора файлов для Android
    if (_controller.platform is AndroidWebViewController) {
      AndroidWebViewController.enableDebugging(true);
      final androidController = _controller.platform as AndroidWebViewController;
      
      androidController
        ..setMediaPlaybackRequiresUserGesture(false)
        ..setGeolocationPermissionsPromptCallbacks(
          onShowPrompt: (request) async {
            return GeolocationPermissionsResponse(
              allow: true,
              retain: true,
            );
          },
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // WebView или экран ошибки
          _errorMessage != null
              ? Container(
                  color: Theme.of(context).scaffoldBackgroundColor,
                  child: Center(
                    child: Padding(
                      padding: const EdgeInsets.all(24.0),
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.error_outline,
                            color: Theme.of(context).colorScheme.error,
                            size: 80,
                          ),
                          const SizedBox(height: 24),
                          Text(
                            _errorMessage!,
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 16,
                              color: Theme.of(context).colorScheme.onSurface,
                            ),
                          ),
                          const SizedBox(height: 32),
                          ElevatedButton.icon(
                            onPressed: () {
                              setState(() {
                                _errorMessage = null;
                              });
                              _controller.reload();
                            },
                            icon: const Icon(Icons.refresh),
                            label: const Text('Обновить страницу'),
                            style: ElevatedButton.styleFrom(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 24,
                                vertical: 12,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                )
              : WebViewWidget(controller: _controller),
          
          // Лоадер поверх WebView при загрузке
          if (_isLoading && !_isFirstLoad)
            Container(
              color: Theme.of(context).scaffoldBackgroundColor.withValues(alpha: 0.8),
              child: Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    CircularProgressIndicator(
                      color: Theme.of(context).colorScheme.primary,
                      strokeWidth: 3,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'Загрузка...',
                      style: TextStyle(
                        fontSize: 16,
                        color: Theme.of(context).colorScheme.onSurface,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          
          // Кнопка для открытия консоли (в правом нижнем углу)
          Positioned(
            right: 16,
            bottom: 16,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Кнопка для тестирования загрузки файлов
                FloatingActionButton(
                  mini: true,
                  backgroundColor: Colors.green.withValues(alpha: 0.9),
                  onPressed: () {
                    // Тестируем клик на input[type="file"]
                    _controller.runJavaScript('''
                      (function() {
                        console.log('🧪 Запущен тест загрузки файлов...');
                        const fileInputs = document.querySelectorAll('input[type="file"]');
                        console.log('Найдено file inputs: ' + fileInputs.length);
                        
                        if (fileInputs.length > 0) {
                          console.log('Пытаемся кликнуть на первый input...');
                          try {
                            fileInputs[0].click();
                            console.log('✅ Клик выполнен успешно');
                          } catch(e) {
                            console.error('❌ Ошибка при клике: ' + e.message);
                          }
                        } else {
                          console.warn('⚠️ Не найдено input[type="file"] на странице');
                          
                          // Ищем другие элементы, которые могут быть кнопками загрузки
                          const allButtons = document.querySelectorAll('button, [role="button"]');
                          console.log('Всего найдено кнопок: ' + allButtons.length);
                          
                          for (let i = 0; i < allButtons.length; i++) {
                            const btn = allButtons[i];
                            const text = btn.textContent || btn.innerText || '';
                            if (text.toLowerCase().includes('загруз') || 
                                text.toLowerCase().includes('upload') ||
                                text.toLowerCase().includes('файл') ||
                                text.toLowerCase().includes('file')) {
                              console.log('Найдена потенциальная кнопка загрузки: "' + text + '"');
                            }
                          }
                        }
                      })();
                    ''');
                  },
                  child: const Icon(
                    Icons.file_upload,
                    color: Colors.white,
                  ),
                  tooltip: 'Тест загрузки',
                ),
                const SizedBox(height: 8),
                // Кнопка консоли
                FloatingActionButton(
                  mini: true,
                  backgroundColor: _showConsole 
                    ? Colors.red.withValues(alpha: 0.9)
                    : Colors.blue.withValues(alpha: 0.9),
                  onPressed: () {
                    setState(() {
                      _showConsole = !_showConsole;
                    });
                  },
                  child: Icon(
                    _showConsole ? Icons.close : Icons.bug_report,
                    color: Colors.white,
                  ),
                  tooltip: _showConsole ? 'Закрыть консоль' : 'Открыть консоль',
                ),
              ],
            ),
          ),
          
          // Консоль JavaScript (выдвигается снизу)
          if (_showConsole)
            Positioned(
              left: 0,
              right: 0,
              bottom: 0,
              height: MediaQuery.of(context).size.height * 0.4,
              child: Container(
                decoration: BoxDecoration(
                  color: Colors.black87,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(16),
                    topRight: Radius.circular(16),
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.5),
                      blurRadius: 10,
                      offset: const Offset(0, -2),
                    ),
                  ],
                ),
                child: Column(
                  children: [
                    // Заголовок консоли
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 12,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.grey[900],
                        borderRadius: const BorderRadius.only(
                          topLeft: Radius.circular(16),
                          topRight: Radius.circular(16),
                        ),
                      ),
                      child: Row(
                        children: [
                          const Icon(
                            Icons.terminal,
                            color: Colors.green,
                            size: 20,
                          ),
                          const SizedBox(width: 8),
                          const Text(
                            'JavaScript Console',
                            style: TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 14,
                            ),
                          ),
                          const Spacer(),
                          IconButton(
                            icon: const Icon(Icons.clear_all, color: Colors.white, size: 20),
                            onPressed: () {
                              setState(() {
                                _consoleLogs.clear();
                              });
                            },
                            tooltip: 'Очистить',
                          ),
                        ],
                      ),
                    ),
                    
                    // Логи
                    Expanded(
                      child: _consoleLogs.isEmpty
                          ? const Center(
                              child: Text(
                                'Нет логов. Взаимодействуйте с сайтом.',
                                style: TextStyle(
                                  color: Colors.grey,
                                  fontSize: 12,
                                ),
                              ),
                            )
                          : ListView.builder(
                              padding: const EdgeInsets.all(8),
                              itemCount: _consoleLogs.length,
                              itemBuilder: (context, index) {
                                final log = _consoleLogs[index];
                                Color textColor = Colors.white;
                                IconData icon = Icons.info_outline;
                                
                                if (log.contains('ERROR')) {
                                  textColor = Colors.red;
                                  icon = Icons.error_outline;
                                } else if (log.contains('WARN')) {
                                  textColor = Colors.orange;
                                  icon = Icons.warning_amber;
                                } else if (log.contains('LOG')) {
                                  textColor = Colors.lightBlue;
                                  icon = Icons.code;
                                }
                                
                                return Padding(
                                  padding: const EdgeInsets.symmetric(vertical: 2),
                                  child: Row(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      Icon(icon, color: textColor, size: 14),
                                      const SizedBox(width: 8),
                                      Expanded(
                                        child: SelectableText(
                                          log,
                                          style: TextStyle(
                                            color: textColor,
                                            fontSize: 11,
                                            fontFamily: 'monospace',
                                          ),
                                        ),
                                      ),
                                    ],
                                  ),
                                );
                              },
                            ),
                    ),
                  ],
                ),
              ),
            ),
        ],
      ),
    );
  }
}
