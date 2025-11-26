using NAudio.Wave;

namespace Waves.Core;

/// <summary>
/// Sample provider that plays back pre-cached audio data.
/// </summary>
public class CachedSoundSampleProvider : ISampleProvider
{
    private readonly CachedSound _cachedSound;
    private long _position;

    public WaveFormat WaveFormat => _cachedSound.WaveFormat;

    public CachedSoundSampleProvider(CachedSound cachedSound)
    {
        _cachedSound = cachedSound;
        _position = 0;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        long availableSamples = _cachedSound.AudioData.Length - _position;
        int samplesToCopy = (int)Math.Min(availableSamples, count);

        Array.Copy(_cachedSound.AudioData, (int)_position, buffer, offset, samplesToCopy);
        _position += samplesToCopy;

        return samplesToCopy;
    }
}
