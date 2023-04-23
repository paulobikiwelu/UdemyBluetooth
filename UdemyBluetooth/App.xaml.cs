namespace UdemyBluetooth;

using UdemyBluetooth.Core;
using Shiny.BluetoothLE.Hosting;
using Shiny.BluetoothLE;

public partial class App : Application
{
	private readonly IBleHostingManager _bleHostingManager;
	private readonly IBleManager _bleManager;

	public App(IBleHostingManager bleHostingManager,
		IBleManager bleManager)
	{
		// Store to the readonly variable (resolved classes/services)
		_bleHostingManager = bleHostingManager;
		_bleManager = bleManager;

		// Initalize the .NET MAUI project and requirements/infrastructure
		InitializeComponent();

		// Load the dependency injection
		Resolver.Build();

		// Load the user interface
		MainPage = new AppShell();
	}

	public IBleHostingManager BluetoothLEHosting => _bleHostingManager;

	public IBleManager BluetoothLE => _bleManager;
}
