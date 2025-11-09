using System.Runtime.InteropServices;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Systems;

/// <summary>
/// System for tracking currently held keys using Windows GetAsyncKeyState.
/// This allows true simultaneous key detection.
/// </summary>
public class InputSystem : IUpdatable
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    // Virtual Key Codes for the keys we care about
    private const int VK_W = 0x57;
    private const int VK_A = 0x41;
    private const int VK_S = 0x53;
    private const int VK_D = 0x44;
    private const int VK_SPACE = 0x20;

    private bool _wasSpacePressed;
    private bool _spaceConsumed;
    private bool _spaceJustPressed;


    /// <summary>
    /// Update order for input processing (0-99 range: Input processing systems).
    /// </summary>
    public int UpdateOrder => 10;

    /// <summary>
    /// Called each game tick to poll keyboard state.
    /// </summary>
    public void Update()
    {
        bool spaceCurrentlyPressed = IsKeyDown(VK_SPACE);
        _spaceJustPressed = spaceCurrentlyPressed && !_wasSpacePressed;
        _spaceConsumed = false;
        _wasSpacePressed = spaceCurrentlyPressed;
    }

    /// <summary>
    /// Checks if spacebar was pressed this frame and consumes it.
    /// This ensures one projectile per press.
    /// </summary>
    public bool WasSpacePressed()
    {
        if (_spaceJustPressed && !_spaceConsumed)
        {
            _spaceConsumed = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a key is currently being held down.
    /// Uses GetAsyncKeyState which returns a short where the high-order bit indicates if the key is down.
    /// </summary>
    private static bool IsKeyDown(int vKey)
    {
        return (GetAsyncKeyState(vKey) & 0x8000) != 0;
    }

    /// <summary>
    /// Gets movement input from WASD keys as a force vector.
    /// </summary>
    /// <param name="force">Base force magnitude.</param>
    /// <returns>Movement force vector.</returns>
    public Vector2 GetMovementInput(float force = 300f)
    {
        Vector2 movement = Vector2.Zero;
        float verticalForce = force * 0.3f;  // 30% of horizontal force
        float horizontalForce = force;

        if (IsKeyDown(VK_W))
            movement += new Vector2(0, -verticalForce);

        if (IsKeyDown(VK_S))
            movement += new Vector2(0, verticalForce);

        if (IsKeyDown(VK_A))
            movement += new Vector2(-horizontalForce, 0);

        if (IsKeyDown(VK_D))
            movement += new Vector2(horizontalForce, 0);

        return movement;
    }
}
