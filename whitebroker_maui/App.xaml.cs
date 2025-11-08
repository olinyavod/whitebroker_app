using WhiteBroker.Services;

namespace WhiteBroker;

public partial class App : Application
{
	public App(CookieManager cookieManager)
	{
		InitializeComponent();

		MainPage = new MainPage(cookieManager);
	}
}





