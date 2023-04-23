using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Interfaces;

namespace UdemyBluetooth.Services
{
    public class MauiInterface : IMauiInterface
    {
        public object Resolve(Type t)
        {
            App context = App.Current as App;

            if (t == typeof(IBleManager))
                return context.BluetoothLE;

            if (t == typeof(IBleHostingManager))
                return context.BluetoothLEHosting;

            return null;
        }
    }
}
