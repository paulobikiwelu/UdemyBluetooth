using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Structures;

namespace UdemyBluetooth.Interfaces
{
    public interface IBluetoothClient
    {
        BluetoothDevice ConnectedDevice { get; }
        ObservableList<BluetoothDevice> ScanResults { get; }
        void StartScan();
        void StopScan();
        bool Connect(BluetoothDevice device);
        bool Disconnect();
        int Average();
    }
}
