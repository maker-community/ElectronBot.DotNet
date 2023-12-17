using Google.Protobuf;
using UsbComm;

namespace HelloWordKeyboard.DotNet;

public static class AssembleDataExtension
{
    public static byte[] EnCodeProtoMessage(this MessageH2D messageH2D)
    {
        var msgBytes = messageH2D.ToByteArray();

        using (MemoryStream ms = new MemoryStream())
        {
            CodedOutputStream output = new CodedOutputStream(ms);
            output.WriteInt32(msgBytes.Length);
            output.Flush();
            byte[] byteList = ms.ToArray();

            var result = byteList.Concat(msgBytes).ToArray();

            return result;
        }
    }
}