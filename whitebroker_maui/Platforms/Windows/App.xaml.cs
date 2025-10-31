using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace WhiteBroker.WinUI;

public partial class App : MauiWinUIApplication
{
	public App()
	{
		this.InitializeComponent();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		if (Application.Windows.Count > 0)
		{
			var currentWindow = Application.Windows[0].Handler?.PlatformView;
			if (currentWindow != null)
			{
				IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(currentWindow);
				var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
				AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
				appWindow.Resize(new SizeInt32(1280, 720));
			}
		}
	}
}

