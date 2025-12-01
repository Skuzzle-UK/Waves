using Waves.Assets.BaseAssets;

namespace Waves.Assets;

/// <summary>
/// Collectable item assets.
/// </summary>
public static class CollectableAssets
{
    /// <summary>
    /// Health pickup - restores 20 health.
    /// </summary>
    public static readonly IAsset HealthUp = new AnimatedAsset(
        0.15f,  // 150ms per frame
        '♥', '❤', '♥', '❤'
    );

    /// <summary>
    /// Invulnerability pickup - grants 10 seconds of invulnerability.
    /// </summary>
    public static readonly IAsset Invulnerability = new AnimatedAsset(
        0.2f,  // 200ms per frame
        '★', '☆', '★', '☆'
    );
}
