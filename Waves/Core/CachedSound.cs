using NAudio.Wave;

namespace Waves.Core;

/// <summary>
/// Stores pre-decoded audio data for fast playback of sound effects.
/// </summary>
public class CachedSound
{
    public byte[] AudioData { get; }
    public WaveFormat WaveFormat { get; }

    public CachedSound(string resourcePath)
    {
        using Stream? stream = System.Reflection.Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource not found: {resourcePath}");
        }

        // Read the complete WAV file into memory (including headers)
        using MemoryStream memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        AudioData = memoryStream.ToArray();

        // Get the wave format by reading from the cached data
        using MemoryStream tempStream = new MemoryStream(AudioData);
        using WaveFileReader reader = new WaveFileReader(tempStream);
        WaveFormat = reader.WaveFormat;
    }
}
