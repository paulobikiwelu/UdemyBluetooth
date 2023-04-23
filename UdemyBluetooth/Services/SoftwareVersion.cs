using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Interfaces;

namespace UdemyBluetooth.Services
{
    public class SoftwareVersion : ISoftwareVersion
    {
        public string Version => @"1.0";
    }
}
