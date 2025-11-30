using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Enemy assets.
/// </summary>
public static class EnemyAssets
{
    public static readonly IAsset Basic = new MultiCharAsset(
        " ▲",
        "╼◉▎",
        " ▼");

    public static readonly IAsset SharkWithLasers = new MultiCharAsset(
        @"╼●╮▗ ",
        @" ≤■■▶❮  "
    );

    public static readonly IAsset WarShip = new MultiCharAsset(
        @" ▁▁▁▁▁▁▁▁",
        @"❮╼●▓╼●▓[▓❱",
        @" ▔▔▔▔▔▔▔▔"
    );

    public static readonly IAsset Kamikaze = new MultiCharAsset(
        @" ╭╽╮",
        @"╼▩█▩╾",
        @" ╰╿╯"
    );
}