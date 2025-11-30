using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Enemy assets.
/// </summary>
public static class EnemyAssets
{
    public static readonly IAsset Basic = new MultiCharAsset("O.o");

    public static readonly IAsset Shark = new MultiCharAsset(
        "  /|",
        "~~~~~~"
    );

    public static readonly IAsset WarShip = new MultiCharAsset(
        @" ▁▁▁▁▁▁▁▁",
        @"❮╼●▓╼●▓[▓❱",
        @" ▔▔▔▔▔▔▔▔"
    );
}