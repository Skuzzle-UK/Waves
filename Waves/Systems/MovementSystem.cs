using Waves.Core.Interfaces;
using Waves.Entities;

namespace Waves.Systems;

/// <summary>
/// System responsible for updating entity movement each game tick.
/// </summary>
public class MovementSystem : IUpdatable
{
    private readonly List<BaseEntity> _entities = [];
    private readonly object _lock = new();
    private const float FixedDeltaTime = 0.016f; // 60 FPS (16ms per frame)

    /// <summary>
    /// Update order for movement processing (300-399 range: Animation and movement systems).
    /// </summary>
    public int UpdateOrder => 350;

    /// <summary>
    /// Registers an entity to be updated by this movement system.
    /// </summary>
    /// <param name="entity">The entity to register.</param>
    public void Register(BaseEntity entity)
    {
        lock (_lock)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }
    }

    /// <summary>
    /// Unregisters an entity from this movement system.
    /// </summary>
    /// <param name="entity">The entity to unregister.</param>
    public void Unregister(BaseEntity entity)
    {
        lock (_lock)
        {
            _entities.Remove(entity);
        }
    }

    /// <summary>
    /// Removes all inactive entities from the system.
    /// </summary>
    public void CleanupInactive()
    {
        lock (_lock)
        {
            _entities.RemoveAll(e => !e.IsActive);
        }
    }

    /// <summary>
    /// Gets the current count of registered entities.
    /// </summary>
    public int EntityCount
    {
        get
        {
            lock (_lock)
            {
                return _entities.Count;
            }
        }
    }

    /// <summary>
    /// Called by GameTick. Updates all registered entities with fixed delta time.
    /// </summary>
    public void Update()
    {
        // Create a snapshot to avoid holding the lock during entity updates
        BaseEntity[] snapshot;
        lock (_lock)
        {
            snapshot = _entities.ToArray();
        }

        // Update all entities with fixed timestep
        foreach (BaseEntity entity in snapshot)
        {
            try
            {
                entity.Update(FixedDeltaTime);
            }
            catch (Exception ex)
            {
                // Log error but continue updating other entities
                Console.WriteLine($"Error updating entity {entity.Id}: {ex.Message}");
            }
        }
    }
}
