namespace Waves.Core.Maths;

/// <summary>
/// Represents a 2D vector with X and Y components.
/// </summary>
public struct Vector2
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the length (magnitude) of the vector.
    /// </summary>
    public readonly float Length => MathF.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Gets the squared length of the vector (faster than Length as it avoids square root).
    /// </summary>
    public readonly float LengthSquared => X * X + Y * Y;

    /// <summary>
    /// Returns a normalized copy of this vector (length = 1).
    /// </summary>
    public readonly Vector2 Normalized()
    {
        float length = Length;
        return length > 0 ? new Vector2(X / length, Y / length) : Zero;
    }

    /// <summary>
    /// Calculates the distance between two vectors.
    /// </summary>
    public static float Distance(Vector2 a, Vector2 b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;

    // Operator overloads
    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);
    public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator *(float scalar, Vector2 a) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);
    public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

    public override readonly bool Equals(object? obj) => obj is Vector2 other && this == other;
    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    public override readonly string ToString() => $"({X}, {Y})";

    // Common vector constants
    public static readonly Vector2 Zero = new(0, 0);
    public static readonly Vector2 One = new(1, 1);
    public static readonly Vector2 Up = new(0, -1);      // Negative Y is up in console coordinates
    public static readonly Vector2 Down = new(0, 1);
    public static readonly Vector2 Left = new(-1, 0);
    public static readonly Vector2 Right = new(1, 0);
}
