
using System.Diagnostics;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using Microsoft.Extensions.Logging;

namespace ElectronBot.DotNet.WinUsb;

/// <summary>
/// 电子SDK接口 / Electron SDK Interface
/// </summary>
public class WinUsbElectronLowLevel : IElectronLowLevel
{
    public string Name => "WinUsb";

    private const int Vid = 0x5241; // Vendor ID

    private const int Pid = 0x5241; // Product ID

    private bool _isConnected = false; // Connection status

    private readonly List<byte[]> _extraDataBufferTx = new() // Transmit buffer for extra data
        {
            new byte[32],
            new byte[32]
        };

    private readonly List<byte[]> _frameBufferTx = new() // Transmit buffer for frame data
        {
            new byte[240 * 240 * 3],
            new byte[240 * 240 * 3]
        };
    private byte[] _extraDataBufferRx = new byte[32]; // Receive buffer for extra data

    private int _pingPongWriteIndex = 0; // Ping-pong buffer index

    private readonly byte[] _usbBuffer200 = new byte[224]; // USB buffer

    private IUsbDevice? _usbDevice; // USB device

    // open read endpoint 1.
    private UsbEndpointReader? _reader; // USB endpoint reader

    // open write endpoint 1.
    private UsbEndpointWriter? _writer; // USB endpoint writer

    private readonly IUsbDevice? _wholeUsbDevice; // Whole USB device

    private readonly UsbContext _context = new(); // USB context

    private readonly ILogger<WinUsbElectronLowLevel> _logger; // Logger


    public static UsbDeviceFinder MyUsbFinder = new() // USB device finder
    {
        Vid = 0x5241,
        Pid = 0x5241
    };

    public WinUsbElectronLowLevel(ILogger<WinUsbElectronLowLevel> logger)
    {
        _logger = logger;
    }

    public bool IsConnected => _isConnected; // Connection status property

    /// <summary>
    /// 连接电子 / Connect Electron
    /// </summary>
    /// <param name="interfaceId">接口id 默认为0可不传 / Interface ID, default is 0</param>
    /// <returns>返回是否成功 / Returns whether the connection was successful</returns>
    public bool Connect(int interfaceId)
    {
        if (_usbDevice == null)
        {
            _context.SetDebugLevel(LibUsbDotNet.LogLevel.Info);

            //Get a list of all connected devices
            //using var usbDeviceCollection = context.List();

            _usbDevice = _context.Find(MyUsbFinder);

            //Narrow down the device by vendor and pid
            //var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == ProductId && d.VendorId == VendorId);

            //Open the device
            _usbDevice.Open();

            //Get the first config number of the interface
            _usbDevice.ClaimInterface(_usbDevice.Configs[0].Interfaces[0].Number);

            _reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            _writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            _isConnected = _usbDevice.IsOpen;

            return _usbDevice.IsOpen;
        }
        else
        {
            return _isConnected;
        }
    }
    /// <summary>
    /// 断开电子 / Disconnect Electron
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the disconnection was successful</returns>
    public bool Disconnect()
    {
        if (_usbDevice != null && _usbDevice.IsOpen)
        {
            _isConnected = false;

            if (_wholeUsbDevice is not null)
            {
                // Release interface #0.
                _wholeUsbDevice.ReleaseInterface(1);
            }

            //_usbDevice.Close();
            _usbDevice.Dispose();
            _context.Dispose();

            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 重置设备 / Reset Device
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the reset was successful</returns>
    public bool ResetDevice()
    {
        var ret = false;

        if (_usbDevice != null && _usbDevice.IsOpen)
        {
            if (_wholeUsbDevice is not null)
            {
                //ret = _wholeUsbDevice.ResetDevice();
            }
        }
        return ret;
    }
    /// <summary>
    /// 获取额外的数据 / Get Extra Data
    /// </summary>
    /// <returns>额外数据的结果 / Extra data result</returns>
    public byte[] GetExtraData()
    {
        var data = new byte[32];

        Array.Copy(_extraDataBufferRx, 0, data, 0, 32);

        return data;
    }
    /// <summary>
    /// 返回舵机的角度列表 / Get Joint Angles
    /// </summary>
    /// <returns>角度列表结果 / List of joint angles</returns>
    public List<float> GetJointAngles()
    {
        var list = new List<float>();

        for (var j = 0; j < 6; j++)
        {
            List<byte> buf = new();

            for (var i = 0; i < 4; i++)
            {
                var temp = _extraDataBufferRx[j * 4 + i + 1];

                buf.Add(temp);
            }

            var angle = BitConverter.ToSingle(buf.ToArray(), 0);

            list.Add(angle);
        }

        return list;
    }
    /// <summary>
    /// 设置额外的数据 / Set Extra Data
    /// </summary>
    /// <param name="data">数据 / Data</param>
    /// <param name="len">数据的长度 / Length of the data</param>
    public void SetExtraData(byte[] data, int len = 32)
    {
        Array.Copy(data, 0, _extraDataBufferTx[_pingPongWriteIndex], 0, len);
    }
    /// <summary>
    /// 设置图片数据 / Set Image Data
    /// </summary>
    /// <param name="data">图片的字节数据 / Byte data of the image</param>
    public void SetImageSrc(byte[] data)
    {
        data.CopyTo(_frameBufferTx[_pingPongWriteIndex], 0);
    }
    /// <summary>
    /// 设置舵机角度 / Set Joint Angles
    /// </summary>
    /// <param name="j1">二号舵机角度 / Angle of joint 2</param>
    /// <param name="j2">四号舵机角度 / Angle of joint 4</param>
    /// <param name="j3">六号舵机角度 / Angle of joint 6</param>
    /// <param name="j4">八号舵机角度 / Angle of joint 8</param>
    /// <param name="j5">十号舵机角度 / Angle of joint 10</param>
    /// <param name="j6">十二号舵机角度 / Angle of joint 12</param>
    /// <param name="enable">是否使能舵机 / Whether to enable the joint</param>
    public void SetJointAngles(float j1, float j2, float j3, float j4, float j5, float j6, bool enable = false)
    {
        var jointAngleSetPoints = new float[6];

        jointAngleSetPoints[0] = j1;
        jointAngleSetPoints[1] = j2;
        jointAngleSetPoints[2] = j3;
        jointAngleSetPoints[3] = j4;
        jointAngleSetPoints[4] = j5;
        jointAngleSetPoints[5] = j6;

        _extraDataBufferTx[_pingPongWriteIndex][0] = Convert.ToByte(enable ? 1 : 0);

        for (var j = 0; j < 6; j++)
        {
            var buf = BitConverter.GetBytes(jointAngleSetPoints[j]);

            for (var i = 0; i < 4; i++)
            {
                _extraDataBufferTx[_pingPongWriteIndex][j * 4 + i + 1] = buf[i];
            }
        }

    }
    /// <summary>
    /// 同步操作数据到电子 / Sync Data to Electron
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the sync was successful</returns>
    public bool Sync()
    {
        if (_isConnected)
        {
            SyncTask();

            return true;
        }
        return false;
    }
    private bool ReceivePacket(int packetCount, int packetSize)
    {
        var stopwatch = Stopwatch.StartNew();

        stopwatch.Start();

        var pCount = packetCount;

        try
        {
            do
            {
                Error ec;
                do
                {
                    var readBuffer = new byte[packetSize];

                    ec = _reader!.Read(readBuffer, 5000, out _);

                    _extraDataBufferRx = readBuffer;

                } while (ec != Error.Success);

                pCount--;

            } while (pCount > 0);

            stopwatch.Stop();

            Console.WriteLine($"time- ReceivePacket time{stopwatch.ElapsedMilliseconds}");

        }
        catch (Exception ex)
        {
            _reader?.Device.Dispose();
            // todo:异常处理 / TODO: Exception handling
        }

        return pCount == 0;
    }
    private bool TransmitPacket(byte[] buffer, int frameBufferOffset, int packetCount, int packetSize)
    {
        var stopwatch = Stopwatch.StartNew();

        stopwatch.Start();

        var pCount = packetCount;

        var dataOffset = 0;

        try
        {
            do
            {
                Error ec;
                do
                {
                    ec = _writer!.Write(buffer, frameBufferOffset + dataOffset, packetSize, 2000, out _);

                    if (packetSize % 512 == 0)
                    {
                        ec = _writer.Write(buffer, frameBufferOffset + dataOffset, 0, 2000, out _);
                    }

                } while (ec != Error.Success);

                dataOffset += packetSize;

                pCount--;

            } while (pCount > 0);

            stopwatch.Stop();

            Console.WriteLine($"time- TransmitPacket time{stopwatch.ElapsedMilliseconds}");
        }
        catch (Exception ex)
        {
            _writer?.Device.Dispose();
            // todo:异常处理 / TODO: Exception handling
        }

        return pCount == 0;
    }
    private void SyncTask()
    {
        var frameBufferOffset = 0;

        var index = _pingPongWriteIndex;

        _pingPongWriteIndex = _pingPongWriteIndex == 0 ? 1 : 0;

        for (var p = 0; p < 4; p++)
        {
            // Wait MCU request & receive 32bytes extra data
            ReceivePacket(1, 32);

            // Transmit buffer
            TransmitPacket(_frameBufferTx[index], frameBufferOffset, 84, 512);

            frameBufferOffset += 43008;

            Array.Copy(_frameBufferTx[index], frameBufferOffset, _usbBuffer200, 0, 192);

            Array.Copy(_extraDataBufferTx[index], 0, _usbBuffer200, 192, 32);

            TransmitPacket(_usbBuffer200, 0, 1, 224);

            frameBufferOffset += 192;
        }
    }
}
