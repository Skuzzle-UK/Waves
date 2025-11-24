using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Enemy assets.
/// </summary>
public static class TerrainAssets
{
    public static readonly IAsset Island1 = new MultiCharAsset(
        @"  ◿▧▩⬛◣",
        @"    ▐ ▲ ₾",
        @"≈≈▟▧▧▮⬛██▙≈≈"
    );

    public static readonly IAsset Island2 = new MultiCharAsset(
        @"         ₾",
        @"≈≈≈▄▆█▇▄▄▟▅█▅▄≈",
        @"≈█▄≈≈≈≈≈≈≈≈≈≈≈≈"
    );

    public static readonly IAsset Island3 = new MultiCharAsset(
        @"  █♣      ▄▆",
        @"≈▄█▇▄≈≈≈≈███▆≈"
    );

    public static readonly IAsset Island4 = new MultiCharAsset(
        @"≈▄█▇▄≈"
    );
    public static readonly IAsset Boat1 = new MultiCharAsset(
        @"  (╈ ◓",
        @"◥▤▤▤▤▤◤"
    );

    public static readonly IAsset Boat2 = new MultiCharAsset(
        @" ☺",
        @"╓╹▤"
    );
}



