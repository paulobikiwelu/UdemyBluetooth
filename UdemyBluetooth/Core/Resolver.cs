using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using UdemyBluetooth.Interfaces;
using UdemyBluetooth.Services;
using AutofacIContainer = Autofac.IContainer;

namespace UdemyBluetooth.Core
{
    public static class Resolver
    {
        private static AutofacIContainer _container;

        public static void Build()
        {
            ContainerBuilder builder = new();

            builder.RegisterType<MauiInterface>().As<IMauiInterface>().SingleInstance();
            builder.RegisterType<SoftwareVersion>().As<ISoftwareVersion>().SingleInstance();
            builder.RegisterType<FileTransfer>().As<IFileTransfer>().SingleInstance();
            builder.RegisterType<BluetoothServer>().As<IBluetoothServer>().SingleInstance();
            builder.RegisterType<BluetoothClient>().As<IBluetoothClient>().SingleInstance();
            
            _container = builder.Build();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
