using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ElectronBot.DotNet
{
    /// <summary>
    /// 电子SDK接口
    /// </summary>
    public class ElectronLowLevel : IElectronLowLevel
    {
        readonly int vid = 0x1001;

        readonly int pid = 0x8023;

        bool isConnected = false;

        List<byte[]> extraDataBufferTx = new()
        {
            new byte[32],
            new byte[32]
        };

        List<byte[]> frameBufferTx = new()
        {
            new byte[240 * 240 * 3],
            new byte[240 * 240 * 3]
        };

        byte[] extraDataBufferRx = new byte[32];

        int pingPongWriteIndex = 0;

        byte[] usbBuffer200 = new byte[224];

        private UsbDevice? _usbDevice;

        // open read endpoint 1.
        private UsbEndpointReader reader;

        // open write endpoint 1.
        private UsbEndpointWriter writer;
        /// <summary>
        /// 连接电子
        /// </summary>
        /// <returns>返回是否成功</returns>
        public bool Connect()
        {
            if (_usbDevice == null)
            {
                var usbFinder = new UsbDeviceFinder(vid, pid);

                _usbDevice = UsbDevice.OpenUsbDevice(usbFinder);

                reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                isConnected = _usbDevice.IsOpen;

                return _usbDevice.IsOpen;
            }
            else
            {
                return isConnected;
            }

        }
        /// <summary>
        /// 断开电子
        /// </summary>
        /// <returns>返回是否成功</returns>
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
        /// <summary>
        /// 获取额外的数据
        /// </summary>
        /// <returns>额外数据的结果</returns>
        public byte[] GetExtraData()
        {
            var data = new byte[32];

            Array.Copy(extraDataBufferRx, 0, data, 0, 32);

            return data;
        }
        /// <summary>
        /// 返回舵机的角度列表
        /// </summary>
        /// <returns>角度列表结果</returns>
        public List<float> GetJointAngles()
        {
            var list = new List<float>();

            for (int j = 0; j < 6; j++)
            {
                List<byte> buf = new();

                for (int i = 0; i < 4; i++)
                {
                    var temp = extraDataBufferRx[j * 4 + i + 1];

                    buf.Add(temp);
                }

                var angle = BitConverter.ToSingle(buf.ToArray(), 0);

                list.Add(angle);
            }

            return list;
        }
        /// <summary>
        /// 设置额外的数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="len">数据的长度</param>

        public void SetExtraData(byte[] data, int len = 32)
        {
            Array.Copy(data, 0, extraDataBufferTx[pingPongWriteIndex], 0, len);
        }
        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="data">图片的字节数据</param>
        public void SetImageSrc(byte[] data)
        {
            data.CopyTo(frameBufferTx[pingPongWriteIndex], 0);
        }
        /// <summary>
        /// 设置舵机角度
        /// </summary>
        /// <param name="j1">二号舵机角度</param>
        /// <param name="j2">四号舵机角度</param>
        /// <param name="j3">六号舵机角度</param>
        /// <param name="j4">八号舵机角度</param>
        /// <param name="j5">十号舵机角度</param>
        /// <param name="j6">十二号舵机角度</param>
        /// <param name="enable">是否使能舵机</param>
        public void SetJointAngles(float j1, float j2, float j3, float j4, float j5, float j6, bool enable = false)
        {
            float[] jointAngleSetPoints = new float[6];

            jointAngleSetPoints[0] = j1;
            jointAngleSetPoints[1] = j2;
            jointAngleSetPoints[2] = j3;
            jointAngleSetPoints[3] = j4;
            jointAngleSetPoints[4] = j5;
            jointAngleSetPoints[5] = j6;

            extraDataBufferTx[pingPongWriteIndex][0] = Convert.ToByte(enable ? 1 : 0);

            for (int j = 0; j < 6; j++)
            {
                var buf = BitConverter.GetBytes(jointAngleSetPoints[j]);

                for (int i = 0; i < 4; i++)
                {
                    extraDataBufferTx[pingPongWriteIndex][j * 4 + i + 1] = buf[i];
                }
            }

        }
        /// <summary>
        /// 同步操作数据到电子
        /// </summary>
        /// <returns>返回是否成功</returns>
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

            int index = this.pingPongWriteIndex;

            this.pingPongWriteIndex = this.pingPongWriteIndex == 0 ? 1 : 0;

            for (int p = 0; p < 4; p++)
            {
                // Wait MCU request & receive 32bytes extra data
                ReceivePacket(1, 32);

                // Transmit buffer
                TransmitPacket(frameBufferTx[index], frameBufferOffset, 84, 512);

                frameBufferOffset += 43008;

                Array.Copy(frameBufferTx[index], frameBufferOffset, usbBuffer200, 0, 192);

                Array.Copy(extraDataBufferTx[index], 0, usbBuffer200, 192, 32);

                TransmitPacket(usbBuffer200, 0, 1, 224);

                frameBufferOffset += 192;
            }
        }
    }
}
