using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Visual assets for scrolling landmass chunks that form environmental boundaries.
/// Top and bottom landmass pieces create a canyon-like environment.
/// </summary>
public static class LandmassAssets
{
    // Top wall chunks (8 characters wide each)

    public static readonly IAsset TopWall1 = new MultiCharAsset(
        "▔▔▔▔▔▔▔▔");

    public static readonly IAsset TopWall2 = new MultiCharAsset(
        "▀▀▀▀▀▀▀▀");

    public static readonly IAsset TopWall3 = new MultiCharAsset(
        "▀▀▀▀▀▀▀▀");

    public static readonly IAsset TopWall4 = new MultiCharAsset(
        "▀▀▀▀▀▀▀▀");

    // Bottom wall chunks (8 characters wide each)

    public static readonly IAsset BottomWall1 = new MultiCharAsset("▄▄▄▄▄▄▄▄");

    public static readonly IAsset BottomWall2 = new MultiCharAsset("▂▂▂▂▂▂▂▂");

    public static readonly IAsset BottomWall3 = new MultiCharAsset("▃▃▃▃▃▃▃▃");

    public static readonly IAsset BottomWall4 = new MultiCharAsset("▃▃▃▂▂▂▂▃");
    

    /// <summary>
    /// Array of all top wall assets for random selection.
    /// </summary>
    public static readonly IAsset[] TopWalls = new[]
    {
        TopWall1,
        TopWall2,
        TopWall3,
        TopWall4
    };

    /// <summary>
    /// Array of all bottom wall assets for random selection.
    /// </summary>
    public static readonly IAsset[] BottomWalls = new[]
    {
        BottomWall1,
        BottomWall2,
        BottomWall3,
        BottomWall4
    };
}
