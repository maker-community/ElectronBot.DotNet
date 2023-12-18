using Google.Protobuf;
using HidApi;
using UsbComm;
using Action = UsbComm.Action;
using DeviceInfo = HelloWordKeyboard.DotNet.Models.DeviceInfo;
using Version = UsbComm.Version;

namespace HelloWordKeyboard.DotNet;

public class Hw75DynamicDevice : IHw75DynamicDevice
{
    private const string Path = "\\\\?\\hid#vid_1d50&pid_615e&mi_01#8&a8a6dc9&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}";

    private const int RePortCount = 63;

    private const int PayloadSize = RePortCount - 1;

    private Device? _device = null;

    public DeviceInfo Open()
    {
        var info = new DeviceInfo();
        try
        {
            _device = new Device(Path);
            if (_device is not null)
            {
                var manufacturer = _device.GetManufacturer();
                var devInfo = _device.GetDeviceInfo();
            }
        }
        catch (Exception)
        {

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

        Task.Delay(20);

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
        return MessageD2H.Parser.ParseFrom(byteList.ToArray());
    }
}