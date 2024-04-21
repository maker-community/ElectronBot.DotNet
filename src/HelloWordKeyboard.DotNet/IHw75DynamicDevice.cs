using HelloWordKeyboard.DotNet.Models;
using UsbComm;

namespace HelloWordKeyboard.DotNet;

public interface IHw75DynamicDevice
{
    /// <summary>
    /// 打开拓展设备
    /// </summary>
    /// <returns></returns>
    DeviceInfo Open();

    /// <summary>
    /// 关闭设备
    /// </summary>
    void Close();

    /// <summary>
    /// 获取设备版本号信息
    /// </summary>
    /// <returns></returns>
    UsbComm.Version GetVersion();

    /// <summary>
    /// 获取设备电机状态
    /// </summary>
    /// <returns></returns>
    UsbComm.MotorState GetMotorState();

    /// <summary>
    /// 设置电机为开关模式
    /// </summary>
    /// <returns></returns>
    UsbComm.MotorState SetKnobSwitchModeConfig(bool demo, KnobConfig.Types.Mode mode);

    /// <summary>
    /// 设置电机模式
    /// </summary>
    /// <returns></returns>
    UsbComm.MotorState SetKnobConfig(UsbComm.KnobConfig config);

    /// <summary>
    /// 设置墨水屏数据
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="partial"></param>
    /// <returns></returns>
    UsbComm.EinkImage SetEInkImage(byte[] imageData, int? x, int? y, int? width, int? height, bool partial = false);
}