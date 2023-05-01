namespace ElectronBot.DotNet;

/// <summary>
/// 电子SDK接口
/// </summary>
public interface IElectronLowLevel
{
    /// <summary>
    /// 是否连接
    /// </summary>
    public bool IsConnected { get; }
    /// <summary>
    /// 连接电子
    /// </summary>
    /// <param name="interfaceId">接口id 默认为0可不传</param>
    /// <returns>返回是否成功</returns>
    bool Connect(int interfaceId = 0);
    /// <summary>
    /// 断开电子
    /// </summary>
    /// <returns>返回是否成功</returns>
    bool Disconnect();

    /// <summary>
    /// 重置设备
    /// </summary>
    /// <returns></returns>
    bool ResetDevice();
    /// <summary>
    /// 同步操作数据到电子
    /// </summary>
    /// <returns>返回是否成功</returns>
    bool Sync();
    /// <summary>
    /// 设置图片数据
    /// </summary>
    /// <param name="data">图片的字节数据</param>
    void SetImageSrc(byte[] data);
    /// <summary>
    /// 设置额外的数据
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="len">数据的长度</param>
    void SetExtraData(byte[] data, int len = 32);
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
    void SetJointAngles(float j1, float j2, float j3, float j4, float j5, float j6, bool enable = false);
    /// <summary>
    /// 返回舵机的角度列表
    /// </summary>
    /// <returns>角度列表结果</returns>
    List<float> GetJointAngles();
    /// <summary>
    /// 获取额外的数据
    /// </summary>
    /// <returns>额外数据的结果</returns>
    byte[] GetExtraData();
}
