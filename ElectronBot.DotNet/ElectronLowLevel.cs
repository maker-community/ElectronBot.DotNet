using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ElectronBot.DotNet
{
    public class ElectronLowLevel : IElectronLowLevel
    {
        readonly int vid = 0x1001;
        readonly int pid = 0x8023;

        bool isConnected = false;

        byte[] extraDataBufferTx = new byte[32];

        byte[] frameBufferTx = new byte[240 * 240 * 3];

        byte[] extraDataBufferRx = new byte[32];

        int pingPongWriteIndex = 0;

        byte[] usbBuffer200 = new byte[224];

        private UsbDevice? _usbDevice;


        // open read endpoint 1.
        private UsbEndpointReader reader;

        // open write endpoint 1.
        private UsbEndpointWriter writer;
        public bool Connect()
        {
            var usbFinder = new UsbDeviceFinder(vid, pid);

            _usbDevice = UsbDevice.OpenUsbDevice(usbFinder);

            reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            isConnected = _usbDevice.IsOpen;

            return _usbDevice.IsOpen;
        }

        public bool Disconnect()
        {
            if (_usbDevice != null && _usbDevice.IsOpen)
            {
                return _usbDevice.Close();
            }
            else
            {
                return false;
            }
        }

        public byte[] GetExtraData()
        {
            return null;
        }

        public List<int> GetJointAngles()
        {
            var list = new List<int>();

            for (int j = 0; j < 6; j++)
            {
                List<byte> buf = new();

                for (int i = 0; i < 4; i++)
                {
                    var temp = extraDataBufferRx[j * 4 + i + 1];

                    buf.Add(temp);
                }

                var angle = BitConverter.ToSingle(buf.ToArray(), 0);

                list.Add((int)angle);
            }

            return list;
        }

        public void SetExtraData(ref byte[] data, int len = 32)
        {
            if (len <= 32)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    extraDataBufferTx[i] = data[i];
                }
            }
        }

        public void SetImageSrc(byte[] data)
        {
            data.CopyTo(frameBufferTx, 0);
        }

        public void SetJointAngles(int j1, int j2, int j3, int j4, int j5, int j6, bool enable = false)
        {
            float[] jointAngleSetPoints = new float[6];

            jointAngleSetPoints[0] = j1;
            jointAngleSetPoints[1] = j2;
            jointAngleSetPoints[2] = j3;
            jointAngleSetPoints[3] = j4;
            jointAngleSetPoints[4] = j5;
            jointAngleSetPoints[5] = j6;

            extraDataBufferTx[0] = Convert.ToByte(enable ? 1 : 0);

            for (int j = 0; j < 6; j++)
            {
                var buf = BitConverter.GetBytes(jointAngleSetPoints[j]);

                for (int i = 0; i < 4; i++)
                {
                    extraDataBufferTx[j * 4 + i + 1] = buf[i];
                }
            }

        }

        public bool Sync()
        {
            if (isConnected)
            {
                SyncTask();

                return true;
            }

            return false;
        }


        private bool ReceivePacket(int packetCount, int packetSize)
        {
            var ec = ErrorCode.Success;

            int _packetCount = packetCount;

            do
            {
                do
                {
                    byte[] readBuffer = new byte[packetSize];

                    ec = reader.Read(readBuffer, 5000, out _);

                    extraDataBufferRx = readBuffer;

                } while (ec != ErrorCode.Success);

                _packetCount--;

            } while (_packetCount > 0);

            return _packetCount == 0;

        }
        private bool TransmitPacket(byte[] buffer, int frameBufferOffset, int packetCount, int packetSize)
        {
            ErrorCode ec = ErrorCode.None;


            int _packetCount = packetCount;

            int dataOffset = 0;

            do
            {
                do
                {
                    ec = writer.Write(buffer, frameBufferOffset + dataOffset, packetSize, 2000, out _);

                    if (packetSize % 512 == 0)
                    {
                        ec = writer.Write(buffer, frameBufferOffset + dataOffset, 0, 2000, out _);
                    }

                } while (ec != ErrorCode.None);

                dataOffset += packetSize;

                _packetCount--;

            } while (_packetCount > 0);

            return _packetCount == 0;
        }
        private void SyncTask()
        {
            int frameBufferOffset = 0;

            int index = pingPongWriteIndex;

            pingPongWriteIndex = pingPongWriteIndex == 0 ? 1 : 0;

            for (int p = 0; p < 4; p++)
            {
                // Wait MCU request & receive 32bytes extra data
                ReceivePacket(1, 32);

                // Transmit buffer
                TransmitPacket(frameBufferTx, frameBufferOffset, 84, 512);

                frameBufferOffset += 43008;

                Array.Copy(frameBufferTx, frameBufferOffset, usbBuffer200, 0, 192);

                Array.Copy(extraDataBufferTx, 0, usbBuffer200, 192, 32);

                TransmitPacket(usbBuffer200, 0, 1, 224);

                frameBufferOffset += 192;
            }
        }
    }
}
