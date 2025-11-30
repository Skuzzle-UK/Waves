namespace Waves.Core.Enums;

/// <summary>
/// Defines the different types of enemy AI behaviors.
/// </summary>
public enum AIType
{
    /// <summary>
    /// Stationary turret that shoots at fixed intervals.
    /// </summary>
    StationaryTurret,

    /// <summary>
    /// Patrol that moves vertically and shoots at the player.
    /// </summary>
    VerticalPatrol,

    /// <summary>
    /// Aggressive chaser that tracks the player's position.
    /// </summary>
    AggressiveChaser
}
