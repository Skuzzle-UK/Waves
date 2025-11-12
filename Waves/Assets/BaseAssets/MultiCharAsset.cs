using Waves.Core.Maths;

namespace Waves.Assets.BaseAssets;

/// <summary>
/// A multi-character asset that can display ASCII art or larger sprites.
/// </summary>
public class MultiCharAsset : IAsset
{
    private readonly char[,] _characters;

    public int Width { get; }
    public int Height { get; }
    public Vector2 Center { get; }

    /// <summary>
    /// Creates a multi-character asset from a 2D array of characters.
    /// </summary>
    /// <param name="characters">2D array where [x,y] represents the character position.</param>
    public MultiCharAsset(char[,] characters)
    {
        _characters = characters ?? throw new ArgumentNullException(nameof(characters));
        Width = characters.GetLength(0);
        Height = characters.GetLength(1);
        Center = new Vector2(Width / 2f, Height / 2f);
    }

    /// <summary>
    /// Creates a multi-character asset from string lines.
    /// </summary>
    /// <param name="lines">Array of strings representing each line of the asset.</param>
    public MultiCharAsset(params string[] lines)
    {
        if (lines == null || lines.Length == 0)
            throw new ArgumentException("Lines cannot be null or empty", nameof(lines));

        Height = lines.Length;
        Width = lines.Max(l => l?.Length ?? 0);

        _characters = new char[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            string line = lines[y] ?? "";
            for (int x = 0; x < Width; x++)
            {
                _characters[x, y] = x < line.Length ? line[x] : ' ';
            }
        }

        Center = new Vector2(Width / 2f, Height / 2f);
    }

    public char GetCharAt(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return _characters[x, y];
        }
        return ' ';
    }

    public void Update(float deltaTime)
    {
        // Static multi-char assets don't animate
    }
}