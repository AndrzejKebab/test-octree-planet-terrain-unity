using UnityEngine;
using Unity.Mathematics;

public static class Tables
{
    public static float TerrainMaxHeight = 32;

    public static readonly Vector3[] Offsets = 
    {
        new(1f, 1f, 1f),
        new(0f, 1f, 1f),
        new(0f, 1f, 0f),
        new(1f, 1f, 0f),
        new(1f, 0f, 1f),
        new(0f, 0f, 1f),
        new(0f, 0f, 0f),
        new(1f, 0f, 0f),
    };

    public static readonly float3 VertexOffset = new(0.5f, 0.5f, 0.5f);

    public static readonly half4[] Vertices = 
    {
        new((half)0, (half)0, (half)0, (half)0),
        new((half)1, (half)0, (half)0, (half)0),
        new((half)1, (half)1, (half)0, (half)0),
        new((half)0, (half)1, (half)0, (half)0),
        new((half)0, (half)0, (half)1, (half)0),
        new((half)1, (half)0, (half)1, (half)0),
        new((half)1, (half)1, (half)1, (half)0),
        new((half)0, (half)1, (half)1, (half)0)
    };

    public static readonly int[][] BuildOrder = new int[][]
    {
        new[] { 1, 2, 5, 6 },
        new[] { 4, 7, 0, 3 },
        new[] { 3, 7, 2, 6 },
        new[] { 1, 5, 0, 4 },
        new[] { 5, 6, 4, 7 },
        new[] { 0, 3, 1, 2 },
    };

    public static readonly int[] VoxelTriangles = 
    {
        1, 2, 5, 6,
        4, 7, 0, 3,
        3, 7, 2, 6,
        1, 5, 0, 4,
        5, 6, 4, 7,
        0, 3, 1, 2
    };

    public static readonly int3[] FaceChecks = 
    {
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 1, 0),
        new(0, -1, 0),
        new(0, 0, 1),
        new(0, 0, -1),
    };
}