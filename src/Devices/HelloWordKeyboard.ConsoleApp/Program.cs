// See https://aka.ms/new-console-template for more information
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using HelloWordKeyboard.DotNet;
using HidApi;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Numerics;

byte[] byteArray = new byte[128 * 296 / 8];

var list = new List<byte>();

var collection = new FontCollection();
var family = collection.Add("./SmileySans-Oblique.ttf");
var font = family.CreateFont(18, FontStyle.Bold);

using (var image = Image.Load<Rgba32>("face.jpg"))
{
    using var overlay = Image.Load<Rgba32>("bzhan.png");
    
    overlay.Mutate(x =>
    {
        x.Resize(new Size(50,50));
    });
    // Convert the image to grayscale
    image.Mutate(x =>
    {
        
        x.DrawImage(overlay,  new Point(0, 64), opacity: 1);
        x.DrawText("粉丝数:", font, Color.Black, new Vector2(20, 220));
        x.DrawText("999999", font, Color.Black, new Vector2(20, 260));
        x.Grayscale();
    });
    
    image.Save("test.jpg");

    byteArray = image.EnCodeImageToBytes();
}

var hidDevice = new Hw75DynamicDevice();

hidDevice.Open();

Stopwatch sw = Stopwatch.StartNew();

sw.Start();

var data111 = hidDevice.SetEInkImage(byteArray, 0, 0, 128, 296, false);

sw.Stop();

Console.WriteLine($"send data ms:{sw.ElapsedMilliseconds}");

Console.ReadKey();

Hid.Exit();


//测试数据十字黑白
// Create a 296*128 matrix
//int[,] matrix = new int[296, 128];

//// Fill the matrix with a cross
//for (int i = 0; i < 296; i++)
//{
//    for (int j = 0; j < 128; j++)
//    {
//        if (i == 296 / 2 || j == 128 / 2)
//        {
//            matrix[i, j] = 1;
//        }
//        else
//        {
//            matrix[i, j] = 0;
//        }
//    }
//}

//// Convert the matrix to a byte array
//byte[] byteArray = new byte[296 * 128 / 8];
//for (int i = 0; i < 296; i++)
//{
//    for (int j = 0; j < 128; j += 8)
//    {
//        for (int k = 0; k < 8; k++)
//        {
//            byteArray[i * 128 / 8 + j / 8] |= (byte)(matrix[i, j + k] << (7 - k));
//        }
//    }
//}