// See https://aka.ms/new-console-template for more information
using Google.Protobuf;
using HelloWordKeyboard.DotNet;
using HidApi;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

//byte[] byteArray = new byte[128 * 296 / 8];

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
var list = new List<byte>();

using (var image = Image.Load<Rgba32>("default.jpg"))
{
    //// Convert the image to grayscale
    image.Mutate(x => x.Grayscale());

    for (int i = 0; i < image.Height; i++)
    {
        for (int j = 0; j < image.Width; j++)
        {
            var buffer32 = image[j, i];

            float grayScale = 0.299f * buffer32.R + 0.587f * buffer32.G + 0.114f * buffer32.B;

            list.Add((byte)grayScale);
        }
    }

    // image.CopyPixelDataTo(byteArray);
}

// Original byte array
byte[] originalArray = list.ToArray();

// New array
byte[] byteArray = new byte[originalArray.Length / 8];

for (int i = 0; i < originalArray.Length; i += 8)
{
    int sum = 0;
    for (int j = 0; j < 8; j++)
    {
        sum += originalArray[i + j];
    }
    byteArray[i / 8] = (byte)(sum / 8);
}

//var byteArray = list.ToArray();

//using (Image<Rgba32> image = new Image<Rgba32>(128, 36))
//{
//    image.Mutate(x => x.BackgroundColor(Color.Black));

//    using (Image<L8> grayImage = image.CloneAs<L8>())
//    {
//        image.CopyPixelDataTo(byteArray);
//        // 将图像转换为字节数组
//        using (MemoryStream ms = new MemoryStream())
//        {
//            grayImage.SaveAsBmp(ms);
//            byte[] bytes1 = ms.ToArray();

//            // 计算二值图像的字节数
//            int byteCount = grayImage.Width * grayImage.Height / 8;
//            Console.WriteLine($"Expected byte count: {byteCount}");
//            Console.WriteLine($"Actual byte count: {bytes1.Length}");
//        }
//    }


//}

var hidDevice = new Hw75DynamicDevice();

hidDevice.Open();

var data111 = hidDevice.SetEInkImage(byteArray, 0, 0, 128, 296, false);

//for (int i = 0; i < byteArray.Length; i+=1)
//{
//    byteArray[i] = 0xff;
//}

//Thread.Sleep(2000);

//var data11122 = hidDevice.SetEInkImage(byteArray, 0, 0, 128, 296, false);

//var data = hidDevice.GetVersion();
//var data11 = hidDevice.GetMotorState();
Console.ReadKey();

var device = new Device("\\\\?\\hid#vid_1d50&pid_615e&mi_01#8&a8a6dc9&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"); //Fill vendor id and product id

Console.WriteLine(device.GetManufacturer());

Console.WriteLine(device.GetDeviceInfo());

//device.SetNonBlocking(true);

var hidMesage = new UsbComm.MessageH2D
{
    Action = UsbComm.Action.Version
};

var bytes = hidMesage.ToByteArray();

var listByte = new List<byte>();
listByte.Add(1);
listByte.Add((byte)(bytes.Length + 1));
listByte.Add((byte)(bytes.Length));
listByte.AddRange(bytes);


listByte.AddRange(new byte[64 - listByte.Count]);

var buffer = listByte.ToArray();

device.Write(buffer);

Task.Delay(20);
var read = device.Read(64);

var result = read[3..(read[2] + 3)];


//var data =  MessageD2H.Parser.ParseFrom(result);
//Console.WriteLine(data);

//Hid.Exit(); //Call at the end of your program
