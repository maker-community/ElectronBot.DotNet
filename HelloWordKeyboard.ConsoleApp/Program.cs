// See https://aka.ms/new-console-template for more information
using HidApi;

var device = new Device("\\\\?\\hid#vid_1d50&pid_615e&mi_01#8&a8a6dc9&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"); //Fill vendor id and product id

Console.WriteLine(device.GetManufacturer());

Console.WriteLine(device.GetDeviceInfo());

device.SetNonBlocking(true);

var buffer = new byte[64];
buffer[0] = 0x01;
buffer[1] = 62;
buffer[2] = 8;
buffer[3] = 1;
buffer[4] = 18;
device.Write(buffer);
var read = device.Read(64);


var buffer1 = new byte[64];
buffer1[0] = 0x01;
buffer1[1] = 62;
buffer1[2] = 8;
buffer1[3] = 1;
buffer1[4] = 18;
device.Write(buffer1);
var read1 = device.Read(64);
Hid.Exit(); //Call at the end of your program
