namespace Waves.Core.Assets;

/// <summary>
/// Predefined game assets for entities.
/// </summary>
public static class GameAssets
{
    /// <summary>
    /// Player assets.
    /// </summary>
    public static class Player
    {
        public static readonly IAsset Surfer = new MultiCharAsset(
            "     o ",
            "   /|\\ ",
            "_,_/_\\___ "
        );
    }

    /// <summary>
    /// Projectile assets.
    /// </summary>
    public static class Projectiles
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

    /// <summary>
    /// Enemy assets.
    /// </summary>
    public static class Enemies
    {
        public static readonly IAsset Basic = new SingleCharAsset('☺');

        public static readonly IAsset UFO = new MultiCharAsset(
            " ▄▄▄ ",
            "▀███▀"
        );

        public static readonly IAsset Spider = new AnimatedAsset(
            0.3f,
            new MultiCharAsset(
                "/◦\\",
                "\\◦/"
            ),
            new MultiCharAsset(
                "\\◦/",
                "/◦\\"
            )
        );
    }
}