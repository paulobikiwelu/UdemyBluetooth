using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Core;
using UdemyBluetooth.Interfaces;
using UdemyBluetooth.Structures;
using SystemTimer = System.Timers.Timer;

namespace UdemyBluetooth.Services
{
    public class BluetoothClient : IBluetoothClient
    {
        private readonly IBleManager _bleManager;
        private readonly ObservableList<BluetoothDevice> _devices = new ObservableList<BluetoothDevice>();

        private BluetoothDevice _connectedDevice;

        public BluetoothClient(IMauiInterface mauiInterface)
        {
            _bleManager = mauiInterface.Resolve(typeof(IBleManager)) as IBleManager;
        }

        public bool Connect(BluetoothDevice device)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            try
            {
                IPeripheral peripheral = (IPeripheral)device.Device;

                peripheral.WhenStatusChanged()
                    .Subscribe(_state =>
                    {
                        if (_state == ConnectionState.Connected)
                        {
                            _connectedDevice = device;
                            _ = tcs.TrySetResult(true);
                        }
                    });

                peripheral.ConnectAsync(timeout: TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _ = tcs.TrySetResult(false);
            }

            return tcs.Task.GetAwaiter().GetResult();
        }

        public bool Disconnect()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            try
            {
                IPeripheral? device = null;

                if (_connectedDevice != null)
                    device = (IPeripheral)_connectedDevice.Device;

                if (!(device!.Status == ConnectionState.Connected))
                {
                    _ = tcs.TrySetResult(false);
                }
                else
                {
                    device.WhenStatusChanged()
                        .Subscribe(_state =>
                        {
                            if (_state == ConnectionState.Disconnected)
                            {
                                _connectedDevice = null;
                                _ = tcs.TrySetResult(true);
                            }
                        });

                    device.CancelConnection();
                }
            }
            catch
            {
                _ = tcs.TrySetResult(false);
            }

            return tcs.Task.GetAwaiter().GetResult();
        }

        public void StartScan()
        {
            if (_bleManager.IsScanning)
            {
                StopScan();
            }

            _devices.Clear();

            _bleManager.Scan()
                .Subscribe(a =>
                {
                    if (_devices.Any(b => b.Uuid.Equals(a.Peripheral.Uuid)))
                        _devices.Remove(_devices.First(b => b.Uuid.Equals(a.Peripheral.Uuid)));

                    if (a.AdvertisementData != null &&
                        a.AdvertisementData.ServiceUuids != null &&
                        a.AdvertisementData.ServiceUuids.Contains(BluetoothConstants.HEART_RATE_SERVICE_UUID.ToString()))
                    {
                        _devices.Add(new BluetoothDevice()
                        {
                            Uuid = a.Peripheral.Uuid,
                            Device = a.Peripheral,
                            LocalName = a.Peripheral.Name,
                            Rssi = a.Rssi
                        });
                    }
                });
        }

        public void StopScan()
        {
            _bleManager.StopScan();
        }

        public int Average()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Task.Run(() =>
            {
                try
                {
                    IPeripheral device = (IPeripheral)_connectedDevice.Device;

                    var characteristic = device.GetKnownCharacteristic(BluetoothConstants.HEART_RATE_SERVICE_UUID.ToString(),
                        BluetoothConstants.HEART_RATE_MEASURE_UUID.ToString(), true).GetAwaiter().GetResult();

                    List<int> heartRates = new List<int>();

                    IDisposable notifications = null;

                    notifications = characteristic.WhenNotificationReceived()
                        .Subscribe(_result =>
                        {
                            if (_result != null && _result.Data != null && _result.Data.Length > 0)
                            {
                                byte[] data = _result.Data;

                                const byte HEART_RATE_VALUE_FORMAT = 0x01;
                                const byte ENERGY_EXPANDED_STATUS = 0x08;

                                byte currentOffset = 0;
                                byte flags = data[currentOffset];
                                bool isHeartRateValueSizeLong = ((flags & HEART_RATE_VALUE_FORMAT) != 0);
                                bool hasEnergyExpended = ((flags & ENERGY_EXPANDED_STATUS) != 0);

                                currentOffset++;

                                ushort heartRateMeasurementValue = 0;

                                if (isHeartRateValueSizeLong)
                                {
                                    heartRateMeasurementValue = (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                                    currentOffset += 2;
                                }
                                else
                                {
                                    heartRateMeasurementValue = data[currentOffset];
                                    currentOffset++;
                                }

                                ushort expendedEnergyValue = 0;

                                if (hasEnergyExpended)
                                {
                                    expendedEnergyValue = (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                                    currentOffset += 2;
                                }

                                if (heartRateMeasurementValue > 0)
                                {
                                    heartRates.Add((int)heartRateMeasurementValue);
                                }
                            }

                            if (heartRates.Count >= 5)
                            {
                                notifications.Dispose();
                                _ = tcs.TrySetResult((int)Math.Round(heartRates.Average(), 0));
                            }
                        });

                    characteristic.Notify(true).GetAwaiter().GetResult();
                }
                catch
                {
                    _ = tcs.TrySetResult(0);
                }
            });

            return tcs.Task.GetAwaiter().GetResult();
        }

        public ObservableList<BluetoothDevice> ScanResults => _devices;

        public BluetoothDevice ConnectedDevice => _connectedDevice;
    }
}
