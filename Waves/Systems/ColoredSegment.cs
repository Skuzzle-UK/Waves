using Spectre.Console;

namespace Waves.Systems;

/// <summary>
/// Represents a segment of text with an optional color.
/// </summary>
public class ColoredSegment
{
    public string Text { get; set; } = string.Empty;
    public Color? Color { get; set; }

    public ColoredSegment(string text, Color? color = null)
    {
        Text = text;
        Color = color;
    }
}
