using Waves.Core.Assets.BaseAssets;

namespace Waves.Core.Assets;

/// <summary>
/// Player assets.
/// </summary>
public static class PlayerAssets
    {
        public static readonly IAsset Surfer = new MultiCharAsset(
            "     o ",
            "   /|\\ ",
            "_,_/_\\___ "
        );
    }
