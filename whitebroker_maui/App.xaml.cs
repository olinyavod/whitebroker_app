using WhiteBroker.Services;

namespace WhiteBroker;

public partial class App : Application
{
	private readonly CookieManager _cookieManager;

	public App(CookieManager cookieManager)
	{
		InitializeComponent();
		_cookieManager = cookieManager;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage(_cookieManager));
	}
}





