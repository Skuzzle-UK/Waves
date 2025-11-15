using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Enemy assets.
/// </summary>
public static class EnemyAssets
{
    public static readonly IAsset Basic = new SingleCharAsset('☺');

    public static readonly IAsset Shark = new MultiCharAsset(
        "  /|",
        "~~~~~~"
    );

    public static readonly IAsset BrickWall = new MultiCharAsset(
        @"╔═══════╗",
        @"║▓▓▓▓▓▓▓║",
        @"╚═══════╝"
    );
}