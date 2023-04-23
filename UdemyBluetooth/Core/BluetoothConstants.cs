using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Extensions;

namespace UdemyBluetooth.Core
{
    public static class BluetoothConstants
    {
        public const string SERVICE_UUID = @"f4d3e542-2ed4-49ec-b9dd-4423a9e44f7c";
        public const string FILE_TRANSFER_UUID = @"db422e2b-7e92-417a-b97f-6313565a9582";

        public static Guid HEART_RATE_SERVICE_UUID = 0x180D.UuidFromPartial();
        public static Guid HEART_RATE_MEASURE_UUID = 0x2A37.UuidFromPartial();
    }
}
