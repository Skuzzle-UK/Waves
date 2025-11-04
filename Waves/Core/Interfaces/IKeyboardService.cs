namespace Waves.Core.Interfaces;

/// <summary>
/// Provides an interface for handling keyboard input in a console application.
/// </summary>
/// <remarks>The <see cref="IKeyboardService"/> interface defines methods and events for managing keyboard
/// interactions, including starting and stopping the service asynchronously and handling key press events.
/// Implementations of this interface can be used to integrate keyboard input handling into console applications,
/// allowing for custom key press actions and service lifecycle management.</remarks>
public interface IKeyboardService
{
    /// <summary>
    /// Occurs when a key is pressed on the console.
    /// </summary>
    /// <remarks>This event is triggered each time a key is pressed, providing the key information through the
    /// <see cref="ConsoleKeyInfo"/> parameter. Subscribers can use this event to handle key press actions in a console
    /// application.</remarks>
    event Action<ConsoleKeyInfo>? OnKeyPressed;

    /// <summary>
    /// Initiates the asynchronous operation to start the service.
    /// </summary>
    /// <remarks>This method should be called to begin the service's operation. It is designed to be
    /// non-blocking and will complete once the service has started.
    /// It is advised that this is not active in menus as it interferes with spectre default key bindings.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Initiates an asynchronous operation to stop the service.
    /// </summary>
    /// <remarks>This method should be called to gracefully stop the service, allowing any ongoing operations
    /// to complete before shutting down.
    /// It is advised that this is not active in menus as it interferes with spectre default key bindings.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken);
}
