using Shiny.BluetoothLE.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Interfaces;
using UdemyBluetooth.Core;

namespace UdemyBluetooth.Services
{
    public class BluetoothServer : IBluetoothServer
    {
        private IGattService _gattService;
        private IGattCharacteristic _gattCharacteristic;

        private readonly IBleHostingManager _bleHostingManager;

        public BluetoothServer(IMauiInterface mauiInterface)
        {
            _bleHostingManager = mauiInterface.Resolve(typeof(IBleHostingManager)) as IBleHostingManager;
        }

        public bool Started => _bleHostingManager.IsAdvertising;

        public void Start()
        {
            if (Started)
                return;

            _bleHostingManager.StartAdvertising();
            CreateService().GetAwaiter().GetResult();
        }

        private void Cleanup()
        {
            if (_gattCharacteristic != null)
            {
                _gattCharacteristic = null;
            }

            if (_gattService != null)
            {
                _gattService = null;
            }
        }

        private async Task CreateService()
        {
            _gattService = await _bleHostingManager.AddService(BluetoothConstants.SERVICE_UUID, true, _builder =>
            {
                CreateCharacteristic(_builder);
            });
        }

        private void CreateCharacteristic(IGattServiceBuilder serviceBuilder)
        {
            _gattCharacteristic = serviceBuilder.AddCharacteristic(BluetoothConstants.FILE_TRANSFER_UUID, _builder =>
            {
                _builder.SetRead(async _request =>
                {
                    var tcs = new TaskCompletionSource<GattResult>();

                    await Task.Run(() =>
                    {
                        _ = tcs.TrySetResult(GattResult.Success(new byte[] { 0x00 }));
                    });

                    return await tcs.Task;
                });

                _builder.SetWrite(async _request =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    await Task.Run(() =>
                    {
                        _ = tcs.TrySetResult(true);
                    });

                    _ = await tcs.Task;
                });
            });
        }

        public void Stop()
        {
            if (!Started)
                return;

            _bleHostingManager.StopAdvertising();

            Cleanup();
        }
    }
}
