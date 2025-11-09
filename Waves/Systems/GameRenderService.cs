using Waves.Core.Interfaces;
using Waves.Entities;

namespace Waves.Systems;

/// <summary>
/// Service for rendering game entities to a character buffer and directly to console.
/// Updates each frame to redraw all entities.
/// </summary>
public class GameRenderService : IUpdatable
{
    private readonly List<BaseEntity> _entities = new();
    private readonly List<Player> _players = new();
    private readonly object _lock = new();

    private char[,] _buffer;
    private int _width;
    private int _height;

    /// <summary>
    /// Update order for rendering (400-499 range: Rendering preparation systems).
    /// </summary>
    public int UpdateOrder => 450;

    public GameRenderService(int width, int height)
    {
        _width = width;
        _height = height;
        _buffer = new char[_width, _height];
        ClearBuffer();
    }

    /// <summary>
    /// Registers an entity to be rendered.
    /// </summary>
    public void RegisterEntity(BaseEntity entity)
    {
        lock (_lock)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }
    }

    /// <summary>
    /// Registers a player to be rendered.
    /// </summary>
    public void RegisterPlayer(Player player)
    {
        // TODO: Probably ties this class into player too much - consider refactoring
        lock (_lock)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
            }
        }
    }

    /// <summary>
    /// Unregisters an entity from rendering.
    /// </summary>
    public void UnregisterEntity(BaseEntity entity)
    {
        lock (_lock)
        {
            _entities.Remove(entity);
        }
    }

    /// <summary>
    /// Unregisters a player from rendering.
    /// </summary>
    public void UnregisterPlayer(Player player)
    {
        lock (_lock)
        {
            _players.Remove(player);
        }
    }

    /// <summary>
    /// Called each game tick to update the render buffer.
    /// </summary>
    public void Update()
    {
        // TODO: Look at refactoring to remove player/projectile ties.
        lock (_lock)
        {
            // Clear buffer
            ClearBuffer();

            // Render players
            foreach (var player in _players)
            {
                if (!player.IsActive)
                {
                    continue;
                }

                // Keep player within bounds
                ClampPositionToBounds(player);

                int x = (int)MathF.Round(player.Position.X);
                int y = (int)MathF.Round(player.Position.Y);

                if (x >= 0 && x < _width && y >= 0 && y < _height)
                {
                    _buffer[x, y] = player.Character;
                }
            }

            // Render other entities (projectiles, etc.)
            foreach (var entity in _entities)
            {
                if (!entity.IsActive)
                {
                    continue;
                }

                // Don't clamp projectiles to bounds - let them go off screen
                // ClampPositionToBounds(entity);

                int x = (int)MathF.Round(entity.Position.X);
                int y = (int)MathF.Round(entity.Position.Y);

                if (x >= 0 && x < _width && y >= 0 && y < _height)
                {
                    // Check if entity is a Projectile to use its DisplayChar
                    char displayChar = '*';
                    if (entity is Projectile projectile)
                    {
                        displayChar = projectile.DisplayChar;
                    }
                    _buffer[x, y] = displayChar;
                }
                else
                {
                    // Deactivate entities that are off screen
                    entity.IsActive = false;
                }
            }
        }
    }

    /// <summary>
    /// Gets the rendered content for a specific row.
    /// </summary>
    public string GetRowContent(int row)
    {
        lock (_lock)
        {
            if (row < 0 || row >= _height)
            {
                return string.Empty;
            }

            var rowString = string.Empty;
            for (int col = 0; col < _width; col++)
            {
                rowString += _buffer[col, row];
            }
            return rowString;
        }
    }

    /// <summary>
    /// Clears the buffer by filling it with spaces.
    /// </summary>
    private void ClearBuffer()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _buffer[x, y] = ' ';
            }
        }
    }

    /// <summary>
    /// Clamps an entity's position to stay within the game bounds.
    /// </summary>
    private void ClampPositionToBounds(BaseEntity entity)
    {
        if (entity.Position.X < 0)
        {
            entity.Position = new Core.Maths.Vector2(0, entity.Position.Y);
        }

        if (entity.Position.X >= _width)
        {
            entity.Position = new Core.Maths.Vector2(_width - 1, entity.Position.Y);
        }

        if (entity.Position.Y < 0)
        {
            entity.Position = new Core.Maths.Vector2(entity.Position.X, 0);
        }

        if (entity.Position.Y >= _height)
        {
            entity.Position = new Core.Maths.Vector2(entity.Position.X, _height - 1);
        }
    }
}
