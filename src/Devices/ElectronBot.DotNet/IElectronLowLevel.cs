namespace ElectronBot.DotNet;

/// <summary>
/// 电子SDK接口 / Electron SDK Interface
/// </summary>
public interface IElectronLowLevel
{
    public string Name
    {
        get;
    }
    /// <summary>
    /// 是否连接 / Is Connected
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// 连接电子 / Connect Electron
    /// </summary>
    /// <param name="interfaceId">接口id 默认为0可不传 / Interface ID, default is 0</param>
    /// <returns>返回是否成功 / Returns whether the connection was successful</returns>
    bool Connect(int interfaceId = 0);

    /// <summary>
    /// 断开电子 / Disconnect Electron
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the disconnection was successful</returns>
    bool Disconnect();

    /// <summary>
    /// 重置设备 / Reset Device
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the reset was successful</returns>
    bool ResetDevice();

    /// <summary>
    /// 同步操作数据到电子 / Sync Data to Electron
    /// </summary>
    /// <returns>返回是否成功 / Returns whether the sync was successful</returns>
    bool Sync();

    /// <summary>
    /// 设置图片数据 / Set Image Data
    /// </summary>
    /// <param name="data">图片的字节数据 / Byte data of the image</param>
    void SetImageSrc(byte[] data);

    /// <summary>
    /// 设置额外的数据 / Set Extra Data
    /// </summary>
    /// <param name="data">数据 / Data</param>
    /// <param name="len">数据的长度 / Length of the data</param>
    void SetExtraData(byte[] data, int len = 32);

    /// <summary>
    /// 设置舵机角度 / Set Joint Angles
    /// </summary>
    /// <param name="j1">二号舵机角度 / Angle of joint 2</param>
    /// <param name="j2">四号舵机角度 / Angle of joint 4</param>
    /// <param name="j3">六号舵机角度 / Angle of joint 6</param>
    /// <param name="j4">八号舵机角度 / Angle of joint 8</param>
    /// <param name="j5">十号舵机角度 / Angle of joint 10</param>
    /// <param name="j6">十二号舵机角度 / Angle of joint 12</param>
    /// <param name="enable">是否使能舵机 / Whether to enable the joint</param>
    void SetJointAngles(float j1, float j2, float j3, float j4, float j5, float j6, bool enable = false);

    /// <summary>
    /// 返回舵机的角度列表 / Get Joint Angles
    /// </summary>
    /// <returns>角度列表结果 / List of joint angles</returns>
    List<float> GetJointAngles();

    /// <summary>
    /// 获取额外的数据 / Get Extra Data
    /// </summary>
    /// <returns>额外数据的结果 / Extra data result</returns>
    byte[] GetExtraData();
}
