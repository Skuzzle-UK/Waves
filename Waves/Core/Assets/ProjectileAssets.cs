using Waves.Core.Assets.BaseAssets;

namespace Waves.Core.Assets;

/// <summary>
/// Projectile assets.
/// </summary>
public static class ProjectileAssets
    {
        public static readonly IAsset Bullet = new SingleCharAsset('•');
        public static readonly IAsset Arrow = new SingleCharAsset('>');
        public static readonly IAsset Laser = new SingleCharAsset('═');

        public static readonly IAsset Missile = new MultiCharAsset("=>>");

        public static readonly IAsset AnimatedBullet = new AnimatedAsset(
            0.1f,  // 100ms per frame
            '•', '○', '◦', '·'
        );

        public static readonly IAsset SpinningProjectile = new AnimatedAsset(
            0.05f,  // 50ms per frame
            '|', '/', '-', '\\'
        );
    }
