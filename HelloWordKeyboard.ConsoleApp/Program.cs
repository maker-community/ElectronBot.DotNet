// See https://aka.ms/new-console-template for more information
using Google.Protobuf;
using HidApi;
using UsbComm;



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


var data =  MessageD2H.Parser.ParseFrom(result);
Console.WriteLine(data);
Hid.Exit(); //Call at the end of your program
