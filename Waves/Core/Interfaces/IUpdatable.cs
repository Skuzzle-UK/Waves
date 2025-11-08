namespace Waves.Core.Interfaces;

/// <summary>
/// Represents a system that receives ordered update calls each game tick.
/// Systems implementing this interface are invoked in a deterministic order
/// based on their UpdateOrder property.
/// </summary>
/// <remarks>
/// Use IUpdatable for core game systems that require predictable execution order
/// (e.g., physics, collision detection, rendering).
/// For less critical systems that just need tick notifications, consider
/// subscribing to IGameTick.OnTick event instead.
/// </remarks>
public interface IUpdatable
{
    /// <summary>
    /// Defines the order in which this system should be updated relative to other IUpdatable systems.
    /// Systems with lower values are updated first.
    /// </summary>
    /// <remarks>
    /// Suggested order ranges:
    /// - 0-99: Input processing systems
    /// - 100-199: Game logic and AI systems
    /// - 200-299: Physics and collision systems
    /// - 300-399: Animation and movement systems
    /// - 400-499: Rendering preparation systems
    /// </remarks>
    int UpdateOrder { get; }

    /// <summary>
    /// Called once per game tick in deterministic order.
    /// Invoked on a background thread pool thread.
    /// </summary>
    void Update();
}
