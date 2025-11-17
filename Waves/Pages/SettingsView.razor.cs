using Microsoft.AspNetCore.Components;
using Waves.Core.Interfaces;

namespace Waves.Pages;

public partial class SettingsView
{
    [Inject]
    private IAudioManager AudioManager { get; set; } = null!;

    [Parameter]
    public EventCallback OnVisibilityChanged { get; set; }

    public bool IsVisible { get; set; }

    public async Task Back()
    {
        IsVisible = false;
        await OnVisibilityChanged.InvokeAsync();
    }

    public async Task Show()
    {
        IsVisible = true;
        await OnVisibilityChanged.InvokeAsync();
    }

    private void IncreaseMusicVolume()
    {
        AudioManager.BackgroundTrackVolume = Math.Min(1.0f, AudioManager.BackgroundTrackVolume + 0.1f);
    }

    private void DecreaseMusicVolume()
    {
        AudioManager.BackgroundTrackVolume = Math.Max(0.0f, AudioManager.BackgroundTrackVolume - 0.1f);
    }

    private void IncreaseSfxVolume()
    {
        AudioManager.OneShotVolume = Math.Min(1.0f, AudioManager.OneShotVolume + 0.1f);
    }

    private void DecreaseSfxVolume()
    {
        AudioManager.OneShotVolume = Math.Max(0.0f, AudioManager.OneShotVolume - 0.1f);
    }

    private string GetMusicVolumeBar()
    {
        int filled = (int)(AudioManager.BackgroundTrackVolume * 10);
        return $"[{new string('█', filled)}{new string('░', 10 - filled)}] {(int)(AudioManager.BackgroundTrackVolume * 100)}%";
    }

    private string GetSfxVolumeBar()
    {
        int filled = (int)(AudioManager.OneShotVolume * 10);
        return $"[{new string('█', filled)}{new string('░', 10 - filled)}] {(int)(AudioManager.OneShotVolume * 100)}%";
    }
}
