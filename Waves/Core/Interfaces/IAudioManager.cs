namespace Waves.Core.Interfaces;

public interface IAudioManager
{
    /// <summary>
    /// Changes the looped background track to the specified embedded audio resource.
    /// This is primarily for background music.
    /// </summary>
    /// <param name="resourcePath">Resource path (e.g., "Assets/Audio/music.wav")</param>
    /// <returns></returns>
    void SetBackgroundTrack(string resourcePath);

    /// <summary>
    /// Starts playing the looped background track if set.
    /// This is primarily for background music.
    /// </summary>
    /// <returns></returns>
    void StartBackgroundTrack();

    /// <summary>
    /// Stops playing the looped background track.
    /// This is primarily for background music.
    /// </summary>
    /// <returns></returns>
    void StopBackgroundTrack();

    /// <summary>
    /// Volume of the loop track. Primarily used for background music.
    /// Can be set from 0.0f to 1.0f, 1.0f being the maximum volume.
    /// </summary>
    float BackgroundTrackVolume {get; set;}

    /// <summary>
    /// Playback speed of the loop track. Primarily used for background music.
    /// Can be set from 1.0f to 2.0f, 1.0f being normal speed.
    /// Values greater than 1.0f speed up playback.
    /// </summary>
    float LoopSpeed { get; set; }

    /// <summary>
    /// Plays an embedded audio resource once.
    /// This is primarily for sound effects.
    /// </summary>
    /// <param name="resourcePath">Resource path (e.g., "Assets/Audio/sound.wav")</param>
    /// <param name="priority">If true, bypasses the concurrent sound effect limit</param>
    /// <returns></returns>
    Task PlayOneShot(string resourcePath, bool priority = false);

    /// <summary>
    /// Volume of the one shot audio tracks. Primarily used for sound effects.
    /// Can be set from 0.0f to 1.0f, 1.0f being the maximum volume.
    /// </summary>
    float OneShotVolume { get; set; }

    Task PreloadAllSoundEffects();

}
