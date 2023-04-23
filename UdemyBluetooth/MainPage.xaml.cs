using UdemyBluetooth.Core;
using UdemyBluetooth.Interfaces;
using UdemyBluetooth.Structures;

namespace UdemyBluetooth;

public partial class MainPage : ContentPage
{
    private bool IsBusy { get; set; } = false;

    public MainPage()
    {
        InitializeComponent();
    }

    /*
    private async void BLEServer_Clicked(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        await Task.Run(() =>
        {
            try
            {
                IBluetoothServer server = Resolver.Resolve<IBluetoothServer>();

                if (server.Started)
                {
                    server.Stop();
                    Dispatcher.Dispatch(() => btnServer.Text = "Start BLE Server");
                }
                else
                {
                    server.Start();
                    Dispatcher.Dispatch(() => btnServer.Text = "Stop BLE Server");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        });
    }
    */

    private async void BLEScan_Clicked(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        await Task.Run(() =>
        {
            try
            {
                IBluetoothClient client = Resolver.Resolve<IBluetoothClient>();
                bool connecting = false;

                client.ScanResults.CollectionChanged += async (s, e) =>
                {
                    if (client.ScanResults.Count > 0 && !connecting)
                    {
                        connecting = true;
                        client.StopScan();

                        await Task.Run(async () =>
                        {
                            IsBusy = true;

                            UdemyBluetooth.Structures.BluetoothDevice device = client.ScanResults.OrderBy(a => a.Rssi)
                                .First();

                            bool result = client.Connect(device);
                            await Task.Delay(TimeSpan.FromSeconds(2));

                            int average = client.Average();
                            System.Diagnostics.Debug.WriteLine($"Heart Rate Average: {average}");

                            await Task.Delay(TimeSpan.FromSeconds(2));
                            result &= client.Disconnect();

                            IsBusy = false;
                        });
                    }
                };

                client.StartScan();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        });
    }
}

