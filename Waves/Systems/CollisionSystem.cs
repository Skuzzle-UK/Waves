using Waves.Core.Collision;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Entities;

namespace Waves.Systems;

public class CollisionSystem : IUpdatable
{
    public int UpdateOrder => GameConstants.UpdateOrder.CollisionDetection;

    private readonly List<ICollidable> _collidables = [];
    private readonly object _lock = new();

    public void RegisterCollidable(ICollidable collidable)
    {
        lock (_lock)
        {
            if (!_collidables.Contains(collidable))
            {
                _collidables.Add(collidable);
            }
        }
    }

    public void UnregisterCollidable(ICollidable collidable)
    {
        lock (_lock)
        {
            _collidables.Remove(collidable);
        }
    }

    public void Update()
    {
        List<ICollidable> snapshot;
        lock (_lock)
        {
            snapshot = new List<ICollidable>(_collidables);
        }

        // O(n²) broad-phase collision detection
        // Can be optimized with spatial partitioning (quadtree/grid) if needed
        for (int i = 0; i < snapshot.Count; i++)
        {
            for (int j = i + 1; j < snapshot.Count; j++)
            {
                CheckAndHandleCollision(snapshot[i], snapshot[j]);
            }
        }
    }

    private void CheckAndHandleCollision(ICollidable a, ICollidable b)
    {
        // Skip disposed or inactive entities (optimization - don't process collisions for disabled entities)
        if (a is BaseEntity entityA && (entityA.IsDisposed || !entityA.IsActive))
        {
            return;
        }
        if (b is BaseEntity entityB && (entityB.IsDisposed || !entityB.IsActive))
        {
            return;
        }

        // Check if layers are configured to collide (bitwise AND with masks)
        bool aCollidesWithB = (a.CollidesWith & b.Layer) != 0;
        bool bCollidesWithA = (b.CollidesWith & a.Layer) != 0;

        if (!aCollidesWithB && !bCollidesWithA)
        {
            return;
        }

        // Perform AABB collision test
        BoundingBox aBounds = a.GetBounds();
        BoundingBox bBounds = b.GetBounds();

        if (aBounds.Intersects(bBounds))
        {
            // Trigger collision callbacks on both entities
            if (aCollidesWithB)
            {
                a.OnCollision(b);
            }
            if (bCollidesWithA)
            {
                b.OnCollision(a);
            }
        }
    }
}
