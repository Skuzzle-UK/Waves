namespace Waves.Core.Interfaces;

/// <summary>
/// Provides methods to control the visibility of the cursor in a console window.
/// </summary>
/// <remarks>This service is used to manage the cursor's visibility state, allowing it to be shown or hidden as
/// needed. It is particularly useful in console applications where cursor visibility needs to be controlled during
/// various operations.</remarks>
public interface ICursorVisibilityService
{
    /// <summary>
    /// Displays the cursor on the screen.
    /// </summary>
    /// <remarks>This method makes the cursor visible if it is currently hidden. It is typically used in
    /// scenarios where the cursor needs to be shown after being hidden by a previous operation.</remarks>
    void ShowCursor();

    /// <summary>
    /// Hides the cursor from the console window.
    /// </summary>
    /// <remarks>This method is typically used in console applications to prevent the cursor from being
    /// visible during operations that do not require user input.</remarks>
    void HideCursor();

    /// <summary>
    /// Gets a value indicating whether the cursor is visible in the window.
    /// </summary>
    bool IsVisible { get; }
}
