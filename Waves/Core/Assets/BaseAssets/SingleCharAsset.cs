using Waves.Core.Maths;

namespace Waves.Core.Assets.BaseAssets;

/// <summary>
/// A simple single-character asset for backward compatibility.
/// </summary>
public class SingleCharAsset : IAsset
{
    private readonly char _character;

    public int Width => 1;
    public int Height => 1;
    public Vector2 Center => new(0.5f, 0.5f);

    public SingleCharAsset(char character)
    {
        _character = character;
    }

    public char GetCharAt(int x, int y)
    {
        return x == 0 && y == 0 ? _character : ' ';
    }

    public void Update(float deltaTime)
    {
        // Single characters don't animate
    }

    /// <summary>
    /// Implicit conversion from char to SingleCharAsset for convenience.
    /// </summary>
    public static implicit operator SingleCharAsset(char c) => new(c);
}