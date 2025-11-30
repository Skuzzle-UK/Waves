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

    public async Task PlayOneShot(string resourcePath)
    {
        // Try to acquire a slot for playing this sound effect
        // If we're already at the limit, skip this sound to prevent stuttering
        bool acquired = await _sfxSemaphore.WaitAsync(0);
        if (!acquired)
        {
            // Too many concurrent sound effects - drop this one
            Debug.WriteLine($"Skipped sound effect (limit reached): {resourcePath}");
            return;
        }

        try
        {
            string resourceName = $"Waves.{resourcePath.Replace('/', '.').Replace('\\', '.')}";

            // Try to get from cache first
            if (!_soundCache.TryGetValue(resourceName, out CachedSound? cachedSound))
            {
                // If not cached, preload it now (fallback - shouldn't normally happen)
                Debug.WriteLine($"Sound effect not in cache, loading on-demand: {resourceName}");
                await PreloadSoundEffect(resourcePath);

                if (!_soundCache.TryGetValue(resourceName, out cachedSound))
                {
                    throw new FileNotFoundException($"Failed to load sound effect: {resourceName}");
                }
            }

            // Play the cached audio from memory
            using (MemoryStream memoryStream = new MemoryStream(cachedSound.AudioData))
            using (WaveFileReader audioFile = new WaveFileReader(memoryStream))
            using (WaveOutEvent outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Volume = OneShotVolume;
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    outputDevice.Volume = OneShotVolume;
                    await Task.Delay(10);
                }
            }
        }
        finally
        {
            // Release the semaphore slot when the sound finishes
            _sfxSemaphore.Release();
        }
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
