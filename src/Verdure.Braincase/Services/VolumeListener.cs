using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace Services
{
    public class VolumeListener
    {
     private MMDevice device;
    
     public event Action<AudioVolumeNotificationData> VolumeChanged;
    
     public VolumeListener()
     {
         MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
         device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
         device.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;
     }
    
     private void OnVolumeNotification(AudioVolumeNotificationData data)
     {
         VolumeChanged?.Invoke(data);
         // 这里是当音量改变时你想要执行的代码
         Console.WriteLine("The volume has been changed to " + data.MasterVolume);
     }
    }
}
