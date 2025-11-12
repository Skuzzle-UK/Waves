namespace Waves.Core.Enums;

[Flags]
public enum CollisionLayer
{
    None = 0,
    Player = 1 << 0,           // 1
    Enemy = 1 << 1,            // 2
    PlayerProjectile = 1 << 2, // 4
    EnemyProjectile = 1 << 3,  // 8
    Obstacle = 1 << 4          // 16
}
