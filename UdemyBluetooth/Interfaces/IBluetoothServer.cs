using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyBluetooth.Interfaces
{
    public interface IBluetoothServer
    {
        bool Started { get; }
        void Start();
        void Stop();
    }
}
