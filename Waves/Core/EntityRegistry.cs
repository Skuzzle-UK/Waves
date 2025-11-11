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
    // Future systems can be added here (CollisionSystem, AudioSystem, etc.)

    private readonly List<BaseEntity> _registeredEntities = new();
    private readonly object _lock = new();

    public EntityRegistry(
        MovementSystem movementSystem,
        GameRenderService renderService)
    {
        _movementSystem = movementSystem ?? throw new ArgumentNullException(nameof(movementSystem));
        _renderService = renderService ?? throw new ArgumentNullException(nameof(renderService));
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

            // Future: Register with other systems based on capabilities
            // if (entity is ICollidable collidable)
            //     _collisionSystem.Register(collidable);
            // if (entity is IAudible audible)
            //     _audioSystem.Register(audible);
        }
    }

    /// <inheritdoc/>
    public void UnregisterEntity(BaseEntity entity)
    {
        if (entity == null) return;

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

            // Future: Unregister from other systems
        }
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
    public void ClearAll()
    {
        lock (_lock)
        {
            // Clear our tracking list
            _registeredEntities.Clear();

            // Note: We don't clear the individual systems here as they
            // may have other entities registered that weren't added through
            // this registry. Systems should handle their own cleanup.
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