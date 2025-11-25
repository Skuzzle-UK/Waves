using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Systems;

/// <summary>
/// Provides keyboard input for the player entity by adapting the InputSystem
/// to the IInputProvider interface.
/// </summary>
public class PlayerInputProvider : IInputProvider
{
    private readonly InputSystem _inputSystem;
    private readonly float _moveForce;

    /// <summary>
    /// Creates a new player input provider.
    /// </summary>
    /// <param name="inputSystem">The underlying input system to read from.</param>
    /// <param name="moveForce">The force to apply for movement (will be normalized).</param>
    public PlayerInputProvider(InputSystem inputSystem, float moveForce)
    {
        _inputSystem = inputSystem ?? throw new ArgumentNullException(nameof(inputSystem));
        _moveForce = moveForce;
    }

    /// <inheritdoc/>
    public Vector2 GetMovementVector()
    {
        // Get the movement input from the input system with the configured force
        Vector2 movement = _inputSystem.GetMovementInput(_moveForce);

        // Normalize to -1 to 1 range if needed (the input system already applies force)
        // For now, we return it as-is since the InputSystem handles the force application
        return movement;
    }

    /// <inheritdoc/>
    public bool IsActionPressed(string action)
    {
        return action?.ToLowerInvariant() switch
        {
            "fire" => _inputSystem.IsSpaceBarPressed(),
            // Add more actions as needed in the future
            // "jump" => _inputSystem.IsKeyPressed(Keys.Space),
            // "dash" => _inputSystem.IsKeyPressed(Keys.Shift),
            _ => false
        };
    }

    /// <inheritdoc/>
    public bool ConsumeAction(string action)
    {
        return action?.ToLowerInvariant() switch
        {
            "fire" => _inputSystem.ConsumeSpaceBar(),
            // Add more consumable actions as needed
            _ => false
        };
    }
}