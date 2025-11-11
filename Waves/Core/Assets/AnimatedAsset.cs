using Waves.Core.Maths;

namespace Waves.Core.Assets;

/// <summary>
/// An animated asset that cycles through multiple frames.
/// </summary>
public class AnimatedAsset : IAsset
{
    private readonly IAsset[] _frames;
    private readonly float _frameTime;
    private float _currentTime;
    private int _currentFrame;

    public int Width => _frames[_currentFrame].Width;
    public int Height => _frames[_currentFrame].Height;
    public Vector2 Center => _frames[_currentFrame].Center;

    /// <summary>
    /// Creates an animated asset from multiple frames.
    /// </summary>
    /// <param name="frameTime">Time in seconds each frame is displayed.</param>
    /// <param name="frames">Array of assets representing animation frames.</param>
    public AnimatedAsset(float frameTime, params IAsset[] frames)
    {
        if (frames == null || frames.Length == 0)
            throw new ArgumentException("Frames cannot be null or empty", nameof(frames));

        _frames = frames;
        _frameTime = frameTime;
        _currentTime = 0f;
        _currentFrame = 0;
    }

    /// <summary>
    /// Creates an animated asset from character frames.
    /// </summary>
    /// <param name="frameTime">Time in seconds each frame is displayed.</param>
    /// <param name="characterFrames">Characters representing animation frames.</param>
    public AnimatedAsset(float frameTime, params char[] characterFrames)
        : this(frameTime, characterFrames.Select(c => new SingleCharAsset(c) as IAsset).ToArray())
    {
    }

    public char GetCharAt(int x, int y)
    {
        return _frames[_currentFrame].GetCharAt(x, y);
    }

    public void Update(float deltaTime)
    {
        _currentTime += deltaTime;

        while (_currentTime >= _frameTime)
        {
            _currentTime -= _frameTime;
            _currentFrame = (_currentFrame + 1) % _frames.Length;
        }

        // Also update the current frame in case it has its own animation
        _frames[_currentFrame].Update(deltaTime);
    }

    /// <summary>
    /// Resets the animation to the first frame.
    /// </summary>
    public void Reset()
    {
        _currentFrame = 0;
        _currentTime = 0f;
    }
}