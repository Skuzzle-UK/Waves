using Waves.Assets.BaseAssets;
using Waves.Entities;

namespace Waves.Assets;

/// <summary>
/// Contains asset definitions for the wave background animation.
/// </summary>
public static class WaveAssets
{
    /// <summary>
    /// Creates an animated wave asset that pulses on the left side of the screen.
    /// The wave spans the full height of the game area.
    /// </summary>
    /// <param name="height">Height of the wave in characters (should match game height).</param>
    /// <returns>An animated asset with two frames that pulse.</returns>
    public static IAsset CreateAnimatedWave(int height)
    {
        // Both frames are 7 chars wide to keep consistent positioning
        // Frame 1: narrower wave with leading space
        var frame1 = CreateWaveFrame(height, 2, true);

        // Frame 2: wider wave
        var frame2 = CreateWaveFrame(height, 4, false);

        return new AnimatedAsset(Wave.FrameTime, frame1, frame2);
    }

    /// <summary>
    /// Creates a single frame of the wave pattern.
    /// Pattern: [optional space] + [light chars...] + ▒ + ▓ + █ + [space]
    /// </summary>
    /// <param name="height">Height of the wave in rows.</param>
    /// <param name="lightCharCount">Number of '░' characters.</param>
    /// <param name="addLeadingSpace">Whether to add a leading space for consistent width.</param>
    /// <returns>A multi-character asset representing one wave frame.</returns>
    private static IAsset CreateWaveFrame(int height, int lightCharCount, bool addLeadingSpace)
    {
        int width = lightCharCount + 4 + (addLeadingSpace ? 1 : 0); // Keep frames same width
        char[,] pattern = new char[width, height];

        for (int row = 0; row < height; row++)
        {
            int col = 0;

            // Add leading char if needed to match widths
            if (addLeadingSpace)
            {
                pattern[col++, row] = '≈';
            }

                pattern[col++, row] = '≈';


            // Add light shade characters
            for (int i = 1; i < lightCharCount - 1; i++)
            {
                pattern[col++, row] = '≈';
            }

            // Add gradient
            pattern[col++, row] = '▒';
            pattern[col++, row] = '▓';
            pattern[col++, row] = '█';
            pattern[col++, row] = '}';
        }

        return new MultiCharAsset(pattern);
    }
}
