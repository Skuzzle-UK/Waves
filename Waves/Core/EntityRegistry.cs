using Waves.Core.Interfaces;
using Waves.Entities;
using Waves.Systems;

namespace Waves.Core;

/// <summary>
/// Central registry that manages entity registration across all game systems.
/// This class decouples entity creation from system knowledge.
/// </summary>
public class EntityRegistry : IEntityRegistry
{
    private readonly MovementSystem _movementSystem;
    private readonly GameRenderService _renderService;
    private readonly CollisionSystem _collisionSystem;
    // Future systems can be added here (AudioSystem, etc.)

    private readonly List<BaseEntity> _registeredEntities = new();
    private readonly object _lock = new();

    public EntityRegistry(
        MovementSystem movementSystem,
        GameRenderService renderService,
        CollisionSystem collisionSystem)
    {
        _movementSystem = movementSystem ?? throw new ArgumentNullException(nameof(movementSystem));
        _renderService = renderService ?? throw new ArgumentNullException(nameof(renderService));
        _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
    }

    /// <inheritdoc/>
    public void RegisterEntity(BaseEntity entity)
    {
        if (entity == null) return;

        lock (_lock)
        {
            // Track the entity
            if (!_registeredEntities.Contains(entity))
            {
                _registeredEntities.Add(entity);
            }

            // Register with movement system (all entities can move)
            _movementSystem.Register(entity);

            // Register with render system if it's renderable
            if (entity is IRenderable renderable)
            {
                _renderService.RegisterRenderable(renderable);
            }

            // Register with collision system if it's collidable
            if (entity is ICollidable collidable)
            {
                _collisionSystem.RegisterCollidable(collidable);
            }

            // Future: Register with other systems based on capabilities
            // if (entity is IAudible audible)
            //     _audioSystem.Register(audible);
        }
    }

    /// <inheritdoc/>
    public void UnregisterEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            return;
        }

        lock (_lock)
        {
            _registeredEntities.Remove(entity);

            // Unregister from movement
            _movementSystem.Unregister(entity);

            // Unregister from rendering
            if (entity is IRenderable renderable)
            {
                _renderService.UnregisterRenderable(renderable);
            }

            // Unregister from collision
            if (entity is ICollidable collidable)
            {
                _collisionSystem.UnregisterCollidable(collidable);
            }

            // Future: Unregister from other systems
        }
    }

    /// <inheritdoc/>
    public void DisposeEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            return;
        }

        // Unregister from all systems first
        UnregisterEntity(entity);

        // Then dispose the entity (marks IsDisposed = true)
        entity.Dispose();
    }

    /// <inheritdoc/>
    public void RegisterForMovement(BaseEntity entity)
    {
        if (entity == null) return;

        lock (_lock)
        {
            _movementSystem.Register(entity);
        }
    }

    /// <inheritdoc/>
    public void RegisterForRendering(IRenderable renderable)
    {
        if (renderable == null) return;

        lock (_lock)
        {
            _renderService.RegisterRenderable(renderable);
        }
    }

    /// <inheritdoc/>
    public void RegisterForCollision(ICollidable collidable)
    {
        if (collidable == null) return;

        lock (_lock)
        {
            _collisionSystem.RegisterCollidable(collidable);
        }
    }

    /// <inheritdoc/>
    public void UnregisterFromCollision(ICollidable collidable)
    {
        if (collidable == null) return;

        lock (_lock)
        {
            _collisionSystem.UnregisterCollidable(collidable);
        }
    }

    /// <inheritdoc/>
    public void ClearAll()
    {
        lock (_lock)
        {
            // Unregister all entities from their systems before clearing
            foreach (var entity in _registeredEntities.ToList())
            {
                // Unregister from movement
                _movementSystem.Unregister(entity);

                // Unregister from rendering
                if (entity is IRenderable renderable)
                {
                    _renderService.UnregisterRenderable(renderable);
                }

                // Unregister from collision
                if (entity is ICollidable collidable)
                {
                    _collisionSystem.UnregisterCollidable(collidable);
                }
            }

            // Clear our tracking list
            _registeredEntities.Clear();
        }
    }

    /// <summary>
    /// Gets the count of currently registered entities.
    /// </summary>
    public int EntityCount
    {
        get
        {
            lock (_lock)
            {
                return _registeredEntities.Count;
            }
        }
    }

    /// <summary>
    /// Checks if an entity is currently registered.
    /// </summary>
    public bool IsRegistered(BaseEntity entity)
    {
        lock (_lock)
        {
            return _registeredEntities.Contains(entity);
        }
    }
}