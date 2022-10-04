using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectronBot.DotNet;
using ElectronBot.BraincasePreview.Core.Models;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace ElectronBot.BraincasePreview.Helpers
{
    public class ElectronBotHelper
    {
        public IElectronLowLevel ElectronBot
        {
            get; set;
        }
        private ElectronBotHelper()
        {
            ElectronBot = App.GetService<IElectronLowLevel>();
        }
        private static ElectronBotHelper? _instance;
        public static ElectronBotHelper Instance => _instance ??= new ElectronBotHelper();

        private readonly SynchronizationContext? _context = SynchronizationContext.Current;

        private readonly Dictionary<string, string> _electronDic = new();

        private DeviceWatcher deviceWatcher;

        public bool EbConnected
        {
            get; set;
        }

        public SerialPort SerialPort { get; set; } = new SerialPort();

        public async Task InitAsync()
        {
            // Target all Serial Devices present on the system
            var deviceSelector = SerialDevice.GetDeviceSelector();

            var myDevices = await DeviceInformation.FindAllAsync(deviceSelector);

            deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);

            deviceWatcher.Start();
        }

        public void PlayEmoticonActionFrame(EmoticonActionFrame frame)
        {
            if (EbConnected)
            {
                try
                {
                    if (frame != null)
                    {
                        ElectronBot.SetImageSrc(frame.FrameBuffer);
                        ElectronBot.SetJointAngles(frame.J1, frame.J2, frame.J3, frame.J4, frame.J5, frame.J6, frame.Enable);
                        ElectronBot.Sync();
                    }   
                }
                catch(Exception ex)
                {
                    return;
                }
               
            }
        }
        private Task ConnectDeviceAsync()
        {
            _context?.Post(async _ =>
            {
                ToastHelper.SendToast("ElectronBotAddTip".GetLocalized(), TimeSpan.FromSeconds(3));

                await Task.Delay(500);
            }, null);

            return Task.CompletedTask;
        }

        private Task DisconnectDeviceAsync()
        {
            _context?.Post(async _ =>
            {
                ToastHelper.SendToast("ElectronBotRemoveTip".GetLocalized(), TimeSpan.FromSeconds(3));
                await Task.Delay(500);
            }, null);

            return Task.CompletedTask;
        }

        private async void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (_electronDic.TryGetValue(args.Id, out var value))
            {
                if (value != null)
                {
                    _electronDic.Remove(args.Id);

                    ElectronBot.Disconnect();

                    EbConnected = false;

                    if (SerialPort.IsOpen)
                    {
                        SerialPort.Close();
                    }


                    await DisconnectDeviceAsync();
                }
            };
        }

        private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            if (args.Name.Contains("CP210"))
            {
                var comName = Regex.Replace(args.Name, @"(.*\()(.*)(\).*)", "$2"); //小括号()

                SerialPort.PortName = comName;

                SerialPort.BaudRate = 115200;

                SerialPort.Open();

                _electronDic.Add(args.Id, args.Name);

                ElectronBot = App.GetService<IElectronLowLevel>();

                EbConnected = ElectronBot.Connect();

                await ConnectDeviceAsync();
            }
        }
    }
}
