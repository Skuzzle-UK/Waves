using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Represents a projectile entity that moves through the game world.
/// </summary>
public class Projectile : BaseEntity
{
    // TODO: Add a asset class which allows for multi-character entities
    /// <summary>
    /// The character or symbol used to display this projectile in the console.
    /// </summary>
    public char DisplayChar { get; set; }

    /// <summary>
    /// Maximum distance this projectile can travel before becoming inactive.
    /// </summary>
    public float MaxDistance { get; set; }

    private readonly Vector2 _startPosition;
    private float _distanceTraveled;

    public Projectile()
    {
        DisplayChar = '*';
        MaxDistance = 1000f;
        Speed = 150f; // Projectiles are faster than default (100)
        _startPosition = Position;
        _distanceTraveled = 0f;
    }

    /// <summary>
    /// Updates the projectile, including distance tracking and deactivation when max distance is reached.
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        // Store old position to calculate distance moved
        Vector2 oldPosition = Position;

        // Apply base movement
        base.Update(deltaTime);

        // Track distance traveled
        _distanceTraveled += Vector2.Distance(oldPosition, Position);

        // Deactivate if max distance exceeded
        if (_distanceTraveled >= MaxDistance)
        {
            IsActive = false;
        }
    }
}
