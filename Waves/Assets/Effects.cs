using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Explosion effects.
/// </summary>
public static class Effects
    {
        public static readonly IAsset SmallExplosion = new AnimatedAsset(
            0.1f,
            new SingleCharAsset('*'),
            new SingleCharAsset('x'),
            new SingleCharAsset('+'),
            new SingleCharAsset('·')
        );

        public static readonly IAsset LargeExplosion = new AnimatedAsset(
            0.15f,
            new MultiCharAsset(
                " * ",
                "*X*",
                " * "
            ),
            new MultiCharAsset(
                "\\|/",
                "-O-",
                "/|\\"
            ),
            new MultiCharAsset(
                "···",
                "·○·",
                "···"
            ),
            new MultiCharAsset(
                "   ",
                " · ",
                "   "
            )
        );
    }
