using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class IndexUtilities
{
    /// <summary>
    /// Converts a 3D location to a 1D index.
    /// </summary>
    /// <param name="xyz">The position of the point in 3D space.</param>
    /// <param name="width">The size of the "container" in the x-direction.</param>
    /// <param name="height">The size of the "container" in the y-direction.</param>
    /// <returns>The 1D representation of the specified location in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XyzToIndex(int3 xyz, int width, int height)
    {
        return XyzToIndex(xyz.x, xyz.y, xyz.z, width, height);
    }

    /// <summary>
    /// Converts a 3D location to a 1D index.
    /// </summary>
    /// <param name="x">The x value of the location.</param>
    /// <param name="y">The y value of the location.</param>
    /// <param name="z">The z value of the location.</param>
    /// <param name="width">The size of the "container" in the x-direction.</param>
    /// <param name="height">The size of the "container" in the y-direction.</param>
    /// <returns>The 1D representation of the specified location in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XyzToIndex(int x, int y, int z, int width, int height)
    {
        return z * width * height + y * width + x;
    }

    /// <summary>
    /// Converts a 1D index to a 3D location.
    /// </summary>
    /// <param name="index">The 1D index in the "container".</param>
    /// <param name="width">The size of the "container" in the x-direction.</param>
    /// <param name="height">The size of the "container" in the y-direction.</param>
    /// <returns>The 3D representation of the specified index in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int3 IndexToXyz(int index, int width, int height)
    {
        int3 position = new(
            index % width,
            index / width % height,
            index / (width * height));
        return position;
    }

    /// <summary>
    /// Converts a 3D location to a 1D index.
    /// </summary>
    /// <param name="xyz">The position of the point in 3D space.</param>
    /// <param name="resolution">The size of the "container" in all directions.</param>
    /// <returns>The 1D representation of the specified location in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XyzToIndex(int3 xyz, int resolution)
    {
        return XyzToIndex(xyz.x, xyz.y, xyz.z, resolution);
    }

    /// <summary>
    /// Converts a 3D location to a 1D index.
    /// </summary>
    /// <param name="x">The x value of the location.</param>
    /// <param name="y">The y value of the location.</param>
    /// <param name="z">The z value of the location.</param>
    /// <param name="resolution">The size of the "container" in all directions.</param>
    /// <returns>The 1D representation of the specified location in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XyzToIndex(int x, int y, int z, int resolution)
    {
        return z * resolution * resolution + y * resolution + x;
    }

    /// <summary>
    /// Converts a 1D index to a 3D location.
    /// </summary>
    /// <param name="index">The 1D index in the "container".</param>
    /// <param name="resolution">The size of the "container" in all directions.</param>
    /// <returns>The 3D representation of the specified index in the "container".</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int3 IndexToXyz(int index, int resolution)
    {
        int3 position = new(
            index % resolution,
            index / resolution % resolution,
            index / (resolution * resolution));
        return position;
    }
}