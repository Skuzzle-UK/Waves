using Waves.Core.Maths;

namespace Waves.Core.Interfaces;

/// <summary>
/// Provides input abstraction for entities, allowing different input sources
/// (keyboard, gamepad, AI, network, etc.) to control game entities.
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// Gets the normalized movement vector for the current frame.
    /// </summary>
    /// <returns>A Vector2 with components between -1 and 1 representing movement direction.</returns>
    Vector2 GetMovementVector();

    /// <summary>
    /// Checks if a named action is currently pressed.
    /// </summary>
    /// <param name="action">The name of the action to check (e.g., "fire", "jump").</param>
    /// <returns>True if the action is currently pressed, false otherwise.</returns>
    bool IsActionPressed(string action);

    /// <summary>
    /// Consumes a one-time action, ensuring it only triggers once per press.
    /// </summary>
    /// <param name="action">The name of the action to consume.</param>
    /// <returns>True if the action was consumed (was pressed and is now consumed), false otherwise.</returns>
    bool ConsumeAction(string action);
}