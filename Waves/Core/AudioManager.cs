using Microsoft.Extensions.Hosting;
using NAudio.Extras;
using NAudio.Wave;
using System.Diagnostics;
using System.Reflection;
using Waves.Assets.Audio;
using Waves.Core.Interfaces;

namespace Waves.Core;

public class AudioManager : IAudioManager, IHostedService
{
    private CancellationToken _applicationLifetime;
    private string _loopTrack = string.Empty;
    private bool _demandLoopPlays = false;
    private bool _newLoopTrackSet = false;
    private float _loopVolume = 1.0f;
    private float _oneShotVolume = 1.0f;
    private float _loopSpeed = 1.0f;
    private SoundTouchSampleProvider? _soundTouchProvider;
    private readonly Dictionary<string, CachedSound> _soundCache = new();
    private bool _soundEffectsPreloaded = false;

    // Limit concurrent sound effects to prevent audio stuttering
    private const int MaxConcurrentSoundEffects = 10;
    private readonly SemaphoreSlim _sfxSemaphore = new(MaxConcurrentSoundEffects, MaxConcurrentSoundEffects);

    public float BackgroundTrackVolume
    {
        get
        {  
            return _loopVolume;
        }
        set
        {
            if (value > 1.0f)
            {
                value = 1.0f;
            }
            if (value < 0.0f)
            {
                value = 0.0f;
            }
            _loopVolume = value;
        }
    }

    public float OneShotVolume
    {
        get
        {
            return _oneShotVolume;
        }
        set
        {
            if (value > 1.0f)
            {
                value = 1.0f;
            }
            if (value < 0.0f)
            {
                value = 0.0f;
            }
            _oneShotVolume = value;
        }
    }

    public float LoopSpeed
    {
        get
        {
            return _loopSpeed;
        }
        set
        {
            if (value > 2f)
            {
                value = 2f;
            }
            if (value < 1f)
            {
                value = 1f;
            }
            _loopSpeed = value;

            // Update the SoundTouch provider if it exists
            if (_soundTouchProvider != null)
            {
                _soundTouchProvider.Tempo = value;
            }
        }
    }

    public async Task PreloadAllSoundEffects()
    {
        if (_soundEffectsPreloaded)
        {
            return; // Already preloaded
        }

        // Use reflection to get all const string fields from AudioResources.SoundEffects
        IEnumerable<FieldInfo> soundEffectFields = typeof(AudioResources.SoundEffects)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

        List<Task> tasks = soundEffectFields
            .Select(field => field.GetValue(null) as string)
            .Where(path => path != null)
            .Select(path => PreloadSoundEffect(path!))
            .ToList();

        await Task.WhenAll(tasks);

        _soundEffectsPreloaded = true;
    }

    private async Task PreloadSoundEffect(string resourcePath)
    {
        string resourceName = $"Waves.{resourcePath.Replace('/', '.').Replace('\\', '.')}";

        if (_soundCache.ContainsKey(resourceName))
        {
            return; // Already cached
        }

        try
        {
            CachedSound cachedSound = new CachedSound(resourceName);
            _soundCache[resourceName] = cachedSound;
            Debug.WriteLine($"Preloaded sound effect: {resourceName}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to preload sound effect {resourceName}: {ex.Message}");
        }
    }

    public async Task PlayOneShot(string resourcePath, bool priority = false)
    {
        bool acquired = false;

        // Priority sounds bypass the limit
        if (!priority)
        {
            // Try to acquire a slot for playing this sound effect
            // If we're already at the limit, skip this sound to prevent stuttering
            acquired = await _sfxSemaphore.WaitAsync(0);
            if (!acquired)
            {
                // Too many concurrent sound effects - drop this one
                Debug.WriteLine($"Skipped sound effect (limit reached): {resourcePath}");
                return;
            }
        }

        string resourceName = $"Waves.{resourcePath.Replace('/', '.').Replace('\\', '.')}";

        // Try to get from cache first
        if (!_soundCache.TryGetValue(resourceName, out CachedSound? cachedSound))
        {
            // If not cached, preload it now (fallback - shouldn't normally happen)
            Debug.WriteLine($"Sound effect not in cache, loading on-demand: {resourceName}");
            await PreloadSoundEffect(resourcePath);

            if (!_soundCache.TryGetValue(resourceName, out cachedSound))
            {
                Debug.WriteLine($"Failed to load sound effect: {resourceName}");
                if (acquired)
                {
                    _sfxSemaphore.Release();
                }
                return;
            }
        }

        // Fire and forget - play the sound without blocking
        _ = Task.Run(() =>
        {
            WaveOutEvent? outputDevice = null;
            try
            {
                // Create a memory stream from cached audio data
                MemoryStream memoryStream = new MemoryStream(cachedSound.AudioData);
                WaveFileReader audioFile = new WaveFileReader(memoryStream);

                // Create output device and set up cleanup on completion
                outputDevice = new WaveOutEvent();

                // Set up automatic disposal when playback stops
                outputDevice.PlaybackStopped += (sender, args) =>
                {
                    audioFile.Dispose();
                    memoryStream.Dispose();
                    outputDevice.Dispose();

                    // Release semaphore slot
                    if (acquired)
                    {
                        _sfxSemaphore.Release();
                    }
                };

                outputDevice.Init(audioFile);
                outputDevice.Volume = OneShotVolume;
                outputDevice.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing sound effect {resourceName}: {ex.Message}");

                // Clean up on error
                outputDevice?.Dispose();

                // Release semaphore on error
                if (acquired)
                {
                    _sfxSemaphore.Release();
                }
            }
        });
    }

    public void SetBackgroundTrack(string resourcePath)
    {
        string resourceName = $"Waves.{resourcePath.Replace('/', '.').Replace('\\', '.')}";
        _loopTrack = resourceName;
        _newLoopTrackSet = true;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_applicationLifetime != cancellationToken)
        {
            _applicationLifetime = cancellationToken;

            return;
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        if (string.IsNullOrWhiteSpace(_loopTrack))
        {
            Debug.WriteLine("ERROR: Loop track has not been set. Set loop track via SetLoopTrack(string) method.");
            return;
        }

        using Stream? stream = assembly.GetManifestResourceStream(_loopTrack);

        if (stream == null)
        {
            Debug.WriteLine($"ERROR: Embedded resource not found: {_loopTrack}.");
            return;
        }

        using (WaveFileReader audioFile = new WaveFileReader(stream))
        using (LoopStream loopStream = new LoopStream(audioFile))
        using (WaveOutEvent outputDevice = new WaveOutEvent())
        {
            // Create and configure SoundTouch for speed adjustment without pitch change
            ISampleProvider sampleProvider = loopStream.ToSampleProvider();
            _soundTouchProvider = new SoundTouchSampleProvider(sampleProvider)
            {
                Tempo = _loopSpeed
            };

            outputDevice.Init(_soundTouchProvider);
            outputDevice.Volume = BackgroundTrackVolume;
            outputDevice.Play();

            while (!_applicationLifetime.IsCancellationRequested && !_newLoopTrackSet && _demandLoopPlays)
            {
                outputDevice.Volume = BackgroundTrackVolume;
                await Task.Delay(10);
            }

            // Clear the reference when playback stops
            _soundTouchProvider = null;
        }

        if (_newLoopTrackSet && !_applicationLifetime.IsCancellationRequested && _demandLoopPlays)
        {
            _newLoopTrackSet = false;
            _ = Task.Run(() => StartAsync(cancellationToken), cancellationToken);
        }
    }

    public void StartBackgroundTrack()
    {
        if (_demandLoopPlays)
        {
            return;
        }

        _demandLoopPlays = true;
        _newLoopTrackSet = false;

        _ = Task.Run(() => StartAsync(_applicationLifetime), _applicationLifetime);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Implement later if needed
        return Task.CompletedTask;
    }

    public void StopBackgroundTrack()
    {
        if (!_demandLoopPlays)
        {
            return;
        }
        
        _demandLoopPlays = false;

        _ = Task.Run(() => StopAsync(_applicationLifetime), _applicationLifetime);
    }
}
