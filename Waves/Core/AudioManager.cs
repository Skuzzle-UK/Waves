using NAudio.Wave;
using Waves.Core.Interfaces;

namespace Waves.Core;

public class AudioManager : IAudioManager
{
    public async Task PlayOnce(string audioFilePath)
    {
        using (var audioFile = new AudioFileReader(audioFilePath))
        using (var outputDevice = new WaveOutEvent())
        {
            outputDevice.Init(audioFile);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
            }
        }
    }

    public Task SetLoopTrack(string audioFilePath)
    {
        throw new NotImplementedException();
    }

    public Task SetLoopVolume(float volume)
    {
        throw new NotImplementedException();
    }

    public Task SetPlayOnceVolume(float volume)
    {
        throw new NotImplementedException();
    }

    public Task StartLoop()
    {
        throw new NotImplementedException();
    }

    public Task StopLoop()
    {
        throw new NotImplementedException();
    }
}
