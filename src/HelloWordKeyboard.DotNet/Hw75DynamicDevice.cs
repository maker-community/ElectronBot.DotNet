using Google.Protobuf;
using HidApi;
using UsbComm;
using Action = UsbComm.Action;
using DeviceInfo = HelloWordKeyboard.DotNet.Models.DeviceInfo;
using Version = UsbComm.Version;

namespace HelloWordKeyboard.DotNet;

public class Hw75DynamicDevice : IHw75DynamicDevice
{
    private const int RePortCount = 63;

    private const int PayloadSize = RePortCount - 1;

    private Device? _device = null;

    private const int ZmkxUasage = 0xff14;

    public DeviceInfo Open()
    {
        var info = new DeviceInfo();
        _device = FindDevice();

        if (_device is not null)
        {
            var devInfo = _device.GetDeviceInfo();

            info.DeviceName = devInfo.ProductString;
            info.Pid = devInfo.ProductId.ToString("X");
            info.Vid = devInfo.VendorId.ToString("X");
        }
        return info;
    }

    public void Close()
    {
        if (_device is not null)
        {
            Hid.Exit();
        }
    }
    public Version GetVersion()
    {
        var version = new MessageH2D()
        {
            Action = Action.Version
        };
        var data = Call(version);

        return data.Version;
    }


    public MotorState SetKnobSwitchModeConfig(bool demo, KnobConfig.Types.Mode mode)
    {
        var switchModeConfig = new KnobConfig()
        {
            Demo = demo,
            Mode = mode
        };
        return SetKnobConfig(switchModeConfig);
    }

    public MotorState SetKnobConfig(KnobConfig config)
    {

        var setKnobConfig = new MessageH2D()
        {
            Action = Action.KnobSetConfig,
            KnobConfig = config
        };
        var data = Call(setKnobConfig);

        return data.MotorState;
    }

    public MotorState GetMotorState()
    {
        var motorGetState = new MessageH2D()
        {
            Action = Action.MotorGetState
        };
        var data = Call(motorGetState);

        return data.MotorState;
    }

    public EinkImage SetEInkImage(byte[] imageData, int? x, int? y, int? width, int? height, bool partial = false)
    {
        _device = FindDevice();
        var eInkImage = new MessageH2D()
        {
            Action = Action.EinkSetImage,
            EinkImage = new EinkImage()
            {
                Id = (uint)new Random().Next() * 1000000,
                Bits = ByteString.CopyFrom(imageData),
                Partial = partial,
            }
        };

        if (x is not null && y is not null && width is not null && height is not null)
        {
            eInkImage.EinkImage.X = (uint)x;
            eInkImage.EinkImage.Y = (uint)y;
            eInkImage.EinkImage.Width = (uint)width;
            eInkImage.EinkImage.Height = (uint)height;
        }

        var data = Call(eInkImage);

        return data.EinkImage;
    }

    private MessageD2H Call(MessageH2D h2d)
    {
        if (_device == null)
        {
            throw new Exception("设备为空");
        }
        var bytes = h2d.EnCodeProtoMessage();

        for (int i = 0; i < bytes.Length; i += PayloadSize)
        {
            var buf = new byte[PayloadSize];

            if (i + PayloadSize > bytes.Length)
            {
                buf = bytes[i..];
            }
            else
            {
                buf = bytes[i..(i + PayloadSize)];
            }

            var list = new byte[2] { 1, (byte)buf.Length };

            var result = list.Concat(buf).ToArray();
            _device.Write(result);
        }

        Task.Delay(100);

        var byteList = new List<byte>();

        while (true)
        {
            var read = _device.Read(RePortCount + 1);
            int cnt = read[1];
            byteList.AddRange(read[3..(cnt + 2)]);
            if (cnt < PayloadSize)
            {
                break;
            }
        }
        try
        {
            var dataResult = MessageD2H.Parser.ParseFrom(byteList.ToArray());

            return dataResult;
        }
        catch (Exception ex)
        {
            throw new Exception("数据解析失败", ex);
        }
    }

    /// <summary>
    /// 获取设备
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Device FindDevice()
    {
        foreach (var deviceInfo in Hid.Enumerate())
        {
            if (deviceInfo.UsagePage == ZmkxUasage)
            {
                _device = new Device(deviceInfo.Path);
                var version = GetVersion();
                if (version.Features.HasEink && version.Features.Eink == true)
                {
                    return new Device(deviceInfo.Path);
                }
            }
        }
        throw new Exception("瀚文拓展设备未连接");
    }
}