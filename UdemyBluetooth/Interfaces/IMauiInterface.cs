﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyBluetooth.Interfaces
{
    public interface IMauiInterface
    {
        object Resolve(Type t);
    }
}
