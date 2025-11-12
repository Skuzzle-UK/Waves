namespace Waves.Core.Collision;

public struct BoundingBox
{
    public float Left { get; set; }
    public float Right { get; set; }
    public float Top { get; set; }
    public float Bottom { get; set; }

    public BoundingBox(float left, float right, float top, float bottom)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }

    /// <summary>
    /// Tests for AABB (Axis-Aligned Bounding Box) intersection
    /// </summary>
    public bool Intersects(BoundingBox other)
    {
        return Left < other.Right &&
               Right > other.Left &&
               Top < other.Bottom &&
               Bottom > other.Top;
    }

    public override string ToString()
    {
        return $"BoundingBox(L:{Left}, R:{Right}, T:{Top}, B:{Bottom})";
    }
}
