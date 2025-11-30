using Waves.Entities;

namespace Waves.Core.Interfaces;

/// <summary>
/// Central registry for managing entity registration across all game systems.
/// Provides a single point of registration instead of coupling to individual systems.
/// </summary>
public interface IEntityRegistry
{
    /// <summary>
    /// Registers an entity with all relevant game systems based on its capabilities.
    /// </summary>
    /// <param name="entity">The entity to register.</param>
    void RegisterEntity(BaseEntity entity);

    /// <summary>
    /// Unregisters an entity from all game systems.
    /// </summary>
    /// <param name="entity">The entity to unregister.</param>
    void UnregisterEntity(BaseEntity entity);

    /// <summary>
    /// Permanently disposes an entity, unregistering it from all systems and calling Dispose().
    /// Use this instead of calling entity.Dispose() directly.
    /// </summary>
    /// <param name="entity">The entity to dispose.</param>
    void DisposeEntity(BaseEntity entity);

    /// <summary>
    /// Registers an entity only with the movement system.
    /// </summary>
    /// <param name="entity">The entity to register for movement.</param>
    void RegisterForMovement(BaseEntity entity);

    /// <summary>
    /// Registers an entity only with the render system.
    /// </summary>
    /// <param name="renderable">The entity to register for rendering.</param>
    void RegisterForRendering(IRenderable renderable);

    /// <summary>
    /// Registers an entity only with the collision system.
    /// </summary>
    /// <param name="collidable">The entity to register for collision detection.</param>
    void RegisterForCollision(ICollidable collidable);

    /// <summary>
    /// Unregisters an entity from the collision system.
    /// </summary>
    /// <param name="collidable">The entity to unregister from collision detection.</param>
    void UnregisterFromCollision(ICollidable collidable);

    /// <summary>
    /// Clears all registered entities from all systems.
    /// Useful for game reset or cleanup.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Clears all enemy projectiles from the game.
    /// Useful for clearing the screen when a boss is defeated.
    /// </summary>
    void ClearEnemyProjectiles();
}