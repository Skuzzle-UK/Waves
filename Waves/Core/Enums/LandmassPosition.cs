namespace Waves.Core.Enums;

/// <summary>
/// Defines the vertical position of a landmass chunk in the game area.
/// </summary>
public enum LandmassPosition
{
    /// <summary>
    /// Top lane landmass (rendered at Y=0).
    /// </summary>
    Top,

    /// <summary>
    /// Bottom lane landmass (rendered at Y=gameHeight-1).
    /// </summary>
    Bottom
}
