using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Player assets.
/// </summary>
public static class PlayerAssets
{
    // TODO: Can we make an AnimatedMultiCharAsset ?
    public static readonly IAsset Surfer = new MultiCharAsset(
        "     o ",
        "   /|\\ ",
        "_,_/_\\___ "
    );
}
