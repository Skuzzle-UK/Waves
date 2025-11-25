using Spectre.Console;
using System.Collections.Generic;
using System.Text;
using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Systems;

/// <summary>
/// Service for rendering game entities to a character buffer and directly to console.
/// Updates each frame to redraw all entities.
/// </summary>
public class GameRenderService : IUpdatable
{
    private readonly List<IRenderable> _renderables = new();
    private readonly object _lock = new();

    private char[,] _buffer;
    private Spectre.Console.Color?[,] _colorBuffer;
    private readonly int _width;
    private readonly int _height;

    /// <summary>
    /// Event fired when rendering is complete and the UI should refresh.
    /// </summary>
    public event Action? RenderComplete;

    /// <summary>
    /// Update order for rendering (400-499 range: Rendering preparation systems).
    /// </summary>
    public int UpdateOrder => GameConstants.UpdateOrder.RenderService;

    public GameRenderService(int width, int height)
    {
        _width = width;
        _height = height;
        _buffer = new char[width, height];
        _colorBuffer = new Spectre.Console.Color?[width, height];
        ClearBuffer();
    }

    /// <summary>
    /// Registers a renderable entity to be displayed.
    /// </summary>
    public void RegisterRenderable(IRenderable renderable)
    {
        lock (_lock)
        {
            if (!_renderables.Contains(renderable))
            {
                _renderables.Add(renderable);
            }
        }
    }

    /// <summary>
    /// Unregisters a renderable entity from display.
    /// </summary>
    public void UnregisterRenderable(IRenderable renderable)
    {
        lock (_lock)
        {
            _renderables.Remove(renderable);
        }
    }

    /// <summary>
    /// Called each game tick to update the render buffer.
    /// </summary>
    public void Update()
    {
        lock (_lock)
        {
            // Clear buffer
            ClearBuffer();

            // Sort renderables by priority (lower priority renders first, higher on top)
            // Filter to only active, non-disposed entities
            List<IRenderable> activeRenderables = _renderables
                .Where(r => r.IsActive && (r is not BaseEntity entity || !entity.IsDisposed))
                .OrderBy(r => r.RenderPriority)
                .ToList();

            // Render each entity
            foreach (IRenderable? renderable in activeRenderables)
            {
                RenderEntity(renderable);
            }

            // Clean up inactive or disposed renderables
            _renderables.RemoveAll(r => !r.IsActive || (r is BaseEntity entity && entity.IsDisposed));
        }

        // Fire event to notify UI to refresh (outside of lock to prevent deadlocks)
        RenderComplete?.Invoke();
    }

    /// <summary>
    /// Renders a single entity to the buffer.
    /// </summary>
    private void RenderEntity(IRenderable renderable)
    {
        // Check if entity has an asset or just a character
        if (renderable.Asset != null)
        {
            RenderAsset(renderable, renderable.Asset);
        }
        else
        {
            RenderSingleChar(renderable);
        }
    }

    /// <summary>
    /// Renders an entity with a multi-character asset.
    /// </summary>
    private void RenderAsset(IRenderable renderable, IAsset asset)
    {
        Vector2 position = renderable.Position;

        // Calculate top-left corner based on asset center
        int startX = (int)MathF.Round(position.X - asset.Center.X);
        int startY = (int)MathF.Round(position.Y - asset.Center.Y);

        // Render each character in the asset
        for (int ay = 0; ay < asset.Height; ay++)
        {
            for (int ax = 0; ax < asset.Width; ax++)
            {
                int worldX = startX + ax;
                int worldY = startY + ay;

                // Apply bounds clamping if required
                if (renderable.ClampToBounds)
                {
                    worldX = Math.Clamp(worldX, 0, _width - 1);
                    worldY = Math.Clamp(worldY, 0, _height - 1);
                }

                // Only render if within bounds
                if (IsInBounds(worldX, worldY))
                {
                    char c = asset.GetCharAt(ax, ay);
                    if (c != ' ') // Don't overwrite with spaces (allows transparency)
                    {
                        _buffer[worldX, worldY] = c;
                        _colorBuffer[worldX, worldY] = renderable.RenderColor;
                    }
                }
            }
        }

        // Check if entity is completely off-screen
        if (!renderable.ClampToBounds)
        {
            bool completelyOffScreen =
                startX + asset.Width < 0 || startX >= _width ||
                startY + asset.Height < 0 || startY >= _height;

            if (completelyOffScreen && renderable is BaseEntity entity)
            {
                entity.IsActive = false;
            }
        }
    }

    /// <summary>
    /// Renders an entity with a single character (backward compatibility).
    /// </summary>
    private void RenderSingleChar(IRenderable renderable)
    {
        Vector2 position = renderable.Position;

        // Apply bounds clamping if required
        if (renderable.ClampToBounds)
        {
            position = ClampPosition(position);
        }

        int x = (int)MathF.Round(position.X);
        int y = (int)MathF.Round(position.Y);

        // Only render if within bounds
        if (IsInBounds(x, y))
        {
            _buffer[x, y] = renderable.DisplayCharacter;
            _colorBuffer[x, y] = renderable.RenderColor;

            // If we had to clamp and the entity wants clamping, update its actual position
            // This maintains the behavior where players are kept in bounds
            if (renderable.ClampToBounds && renderable is BaseEntity entity)
            {
                entity.Position = position;
            }
        }
        else if (!renderable.ClampToBounds)
        {
            // For entities that don't clamp (like projectiles), deactivate when off-screen
            if (renderable is BaseEntity entity)
            {
                entity.IsActive = false;
            }
        }
    }

    /// <summary>
    /// Clamps a position to stay within the game bounds.
    /// </summary>
    private Vector2 ClampPosition(Vector2 position)
    {
        float x = Math.Clamp(position.X, 0, _width - 1);
        float y = Math.Clamp(position.Y, 0, _height - 1);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Checks if coordinates are within the render bounds.
    /// </summary>
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    /// <summary>
    /// Gets the rendered content as structured lines with colored segments.
    /// Each line contains segments that may have different colors.
    /// </summary>
    public List<List<ColoredSegment>> GetRenderedLines()
    {
        lock (_lock)
        {
            List<List<ColoredSegment>> lines = new List<List<ColoredSegment>>();

            for (int row = 0; row < _height; row++)
            {
                List<ColoredSegment> segments = new List<ColoredSegment>();
                Spectre.Console.Color? currentColor = null;
                StringBuilder currentSegment = new System.Text.StringBuilder();

                for (int col = 0; col < _width; col++)
                {
                    Color? cellColor = _colorBuffer[col, row];
                    char cellChar = _buffer[col, row];

                    // Check if color changed
                    if (cellColor != currentColor)
                    {
                        // Save previous segment if it has content
                        if (currentSegment.Length > 0)
                        {
                            segments.Add(new ColoredSegment(currentSegment.ToString(), currentColor));
                            currentSegment.Clear();
                        }
                        currentColor = cellColor;
                    }

                    currentSegment.Append(cellChar);
                }

                // Add final segment for this line
                if (currentSegment.Length > 0)
                {
                    segments.Add(new ColoredSegment(currentSegment.ToString(), currentColor));
                }

                lines.Add(segments);
            }

            return lines;
        }
    }

    /// <summary>
    /// Gets the complete rendered content as a single string with ANSI color codes.
    /// </summary>
    public string GetRenderedContent()
    {
        lock (_lock)
        {
            StringBuilder content = new System.Text.StringBuilder(_width * _height + _height);

            for (int row = 0; row < _height; row++)
            {
                Spectre.Console.Color? currentColor = null;
                bool needsReset = false;

                for (int col = 0; col < _width; col++)
                {
                    Color? cellColor = _colorBuffer[col, row];
                    char cellChar = _buffer[col, row];

                    // Check if we need to change color
                    if (cellColor != currentColor)
                    {
                        // Only add color codes if there's an actual color
                        if (cellColor.HasValue)
                        {
                            content.Append(GetAnsiColorCode(cellColor.Value));
                            needsReset = true;
                        }
                        else if (needsReset)
                        {
                            // Reset to default only if we previously set a color
                            content.Append($"{(char)27}[0m");
                            needsReset = false;
                        }

                        currentColor = cellColor;
                    }

                    content.Append(cellChar);
                }

                // Reset at end of line only if needed
                if (needsReset)
                {
                    content.Append($"{(char)27}[0m");
                }

                // Add newline except for the last row
                if (row < _height - 1)
                {
                    content.AppendLine();
                }
            }

            return content.ToString();
        }
    }

    /// <summary>
    /// Converts a Spectre.Console color to an ANSI escape code.
    /// </summary>
    private static string GetAnsiColorCode(Spectre.Console.Color color)
    {
        // Using (char)27 instead of \x1b to avoid preprocessing issues
        char esc = (char)27;

        // Map common colors to ANSI codes (foreground colors)
        if (color == Spectre.Console.Color.Red || color == Spectre.Console.Color.Red1)
            return $"{esc}[31m";
        if (color == Spectre.Console.Color.Green)
            return $"{esc}[32m";
        if (color == Spectre.Console.Color.Blue)
            return $"{esc}[34m";
        if (color == Spectre.Console.Color.Yellow)
            return $"{esc}[33m";

        // Default: no color
        return string.Empty;
    }

    /// <summary>
    /// Clears the buffer by filling it with spaces and removing colors.
    /// </summary>
    private void ClearBuffer()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _buffer[x, y] = GameConstants.Display.EmptyChar;
                _colorBuffer[x, y] = null;
            }
        }
    }
}