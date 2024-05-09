using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace Services
{
    public class VolumeListener
    {
        private WasapiOut _output;
        private MMDevice _device;

        public event Action<AudioVolumeNotificationData> VolumeChanged;

        public VolumeListener()
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            _device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _output = new WasapiOut(_device, AudioClientShareMode.Shared, true, 100);
            _output.PlaybackStopped += OnPlaybackStopped;
        }

        public void StartListening()
        {
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;
            _output.Play();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            _output.Dispose();
        }

        private void OnVolumeNotification(AudioVolumeNotificationData data)
        {
            VolumeChanged?.Invoke(data);
            // 这里是当音量改变时你想要执行的代码
            Console.WriteLine("The volume has been changed to " + data.MasterVolume);
        }
    }
}
