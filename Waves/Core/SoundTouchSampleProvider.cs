using NAudio.Wave;
using SoundTouchSharp;

namespace Waves.Core;

public class SoundTouchSampleProvider : ISampleProvider
{
    private readonly ISampleProvider _sourceProvider;
    private readonly SoundTouch _soundTouch;
    private readonly int _channelCount;
    private float _tempo = 1.0f;
    private readonly float[] _sourceBuffer;
    private readonly float[] _outputBuffer;
    private const int BufferSize = 4096;

    public WaveFormat WaveFormat { get; }

    public float Tempo
    {
        get => _tempo;
        set
        {
            _tempo = value;
            // SoundTouch uses tempo as a percentage change, so 1.0 = 0% change, 1.5 = 50% faster, 0.5 = 50% slower
            _soundTouch.Tempo = value;
        }
    }

    public SoundTouchSampleProvider(ISampleProvider sourceProvider)
    {
        _sourceProvider = sourceProvider;
        WaveFormat = sourceProvider.WaveFormat;
        _channelCount = WaveFormat.Channels;

        _soundTouch = new SoundTouch();
        _soundTouch.SampleRate = (uint)WaveFormat.SampleRate;
        _soundTouch.Channels = (uint)_channelCount;
        _soundTouch.Tempo = _tempo;

        _sourceBuffer = new float[BufferSize * _channelCount];
        _outputBuffer = new float[BufferSize * _channelCount];
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = 0;

        while (samplesRead < count)
        {
            int samplesRemaining = (count - samplesRead) / _channelCount;
            int maxSamples = _outputBuffer.Length / _channelCount;
            uint numSamples = (uint)Math.Min(samplesRemaining, maxSamples);

            int receivedSamples = (int)_soundTouch.ReceiveSamples(_outputBuffer, numSamples);

            if (receivedSamples > 0)
            {
                int samplesToCopy = receivedSamples * _channelCount;
                for (int i = 0; i < samplesToCopy; i++)
                {
                    buffer[offset + samplesRead + i] = _outputBuffer[i];
                }
                samplesRead += samplesToCopy;
            }
            else
            {
                int sourceSamplesNeeded = Math.Min(_sourceBuffer.Length, (count - samplesRead) * 2);
                int sourceSamplesRead = _sourceProvider.Read(_sourceBuffer, 0, sourceSamplesNeeded);

                if (sourceSamplesRead == 0)
                {
                    _soundTouch.Flush();
                    receivedSamples = (int)_soundTouch.ReceiveSamples(_outputBuffer, numSamples);

                    if (receivedSamples > 0)
                    {
                        int samplesToCopy = receivedSamples * _channelCount;
                        for (int i = 0; i < samplesToCopy; i++)
                        {
                            buffer[offset + samplesRead + i] = _outputBuffer[i];
                        }
                        samplesRead += samplesToCopy;
                    }

                    break;
                }

                _soundTouch.PutSamples(_sourceBuffer, (uint)(sourceSamplesRead / _channelCount));
            }
        }

        return samplesRead;
    }
}
