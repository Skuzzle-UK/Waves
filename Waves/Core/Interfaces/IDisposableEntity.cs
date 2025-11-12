namespace Waves.Core.Interfaces;

/// <summary>
/// Represents an entity that can be permanently disposed of and removed from the game.
/// This is distinct from IsActive which represents temporary enable/disable state.
/// </summary>
public interface IDisposableEntity : IDisposable
{
    /// <summary>
    /// Whether this entity has been disposed and should be removed from all systems.
    /// Once disposed, an entity cannot be reused.
    /// </summary>
    bool IsDisposed { get; }
}
