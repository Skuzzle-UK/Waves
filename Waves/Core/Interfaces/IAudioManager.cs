namespace Waves.Core.Interfaces;

public interface IAudioManager
{
    /// <summary>
    /// Changes the looped background track to the specified audio file.
    /// This is primarily for background music.
    /// </summary>
    /// <param name="track"></param>
    /// <returns></returns>
    Task SetLoopTrack(string audioFilePath);

    /// <summary>
    /// Starts playing the looped background track if set.
    /// This is primarily for background music.
    /// </summary>
    /// <returns></returns>
    Task StartLoop();

    /// <summary>
    /// Stops playing the looped background track.
    /// This is primarily for background music.
    /// </summary>
    /// <returns></returns>
    Task StopLoop();

    /// <summary>
    /// Change the volume of the background loop track.
    /// This is primarily for background music.
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    Task SetLoopVolume(float volume);

    /// <summary>
    /// Plays an audio file once.
    /// This is primarily for sound effects.
    /// </summary>
    /// <param name="audioFilePath"></param>
    /// <returns></returns>
    Task PlayOnce(string audioFilePath);

    /// <summary>
    /// Set the volume for one shot audio.
    /// This is primarily for sound effects.
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    Task SetPlayOnceVolume(float volume);
}
