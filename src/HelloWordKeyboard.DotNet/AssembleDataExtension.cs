using Google.Protobuf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

    public static byte[] EnCodeImageToBytes(this Image<Rgba32> image)
    {
        // Create a 01 matrix
        int[,] matrix = new int[image.Height, image.Width];

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                matrix[y, x] = (image[x, y].R + image[x, y].G + image[x, y].B) / 3 > 128 ? 1 : 0;
            }
        }

        // Convert the matrix to a byte array
        byte[] byteArray = new byte[image.Height * image.Width / 8];
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x += 8)
            {
                for (int k = 0; k < 8; k++)
                {
                    byteArray[y * image.Width / 8 + x / 8] |= (byte)(matrix[y, x + k] << (7 - k));
                }
            }
        }

        return byteArray;
    }
}