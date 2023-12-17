using Google.Protobuf;
using UsbComm;

namespace HelloWordKeyboard.DotNet;

public static class AssembleDataExtension
{
    public static byte[] EnCodeProtoMessage(this MessageH2D messageH2D)
    {
        var msgBytes = messageH2D.ToByteArray();

        var list = new List<byte> { (byte)msgBytes.Length };

        list.AddRange(msgBytes);
        
        return list.ToArray();
    }
}