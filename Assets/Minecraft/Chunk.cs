using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using System;

[Serializable]
public class Chunk
{
    public const int Width = 16;
    public const int Height = 256;
    public const int Count = Width * Height * Width;
    public static readonly Bounds Bounds = new(new Vector3(Width, Height, Width) * 0.5f,
        new Vector3(Width, Height, Width));

    public NativeArray<byte> Blocks;
    [field:SerializeField]public Bounds Boundary { get; private set; }
    private Minecraft minecraft;
    private Mesh mesh;

    public Chunk(Minecraft minecraft, int3 coord)
    {
        var position = new Vector3(coord.x * Width, 0, coord.z * Width);
        var size = new Vector3(Width, Height, Width);
        var center = (size / 2f) - (Vector3.one / 2f);
        Boundary = new Bounds(center + position, size);
        this.minecraft = minecraft;
    }

    ~Chunk()
    {
        Blocks.Dispose();
    }

    public void Draw()
    {
        if (!mesh)
        {
            if (!GeometryUtility.TestPlanesAABB(minecraft.Frustrum, Boundary)) return;
            Schedule(out var completer);
            minecraft.ToComplete.Add(completer);

            return;
        }

        Graphics.DrawMesh(mesh, Boundary.min, Quaternion.identity, minecraft.Material, 0);
    }

    private void Schedule(out JobCompleter jobCompleter)
    {
        var vertexData =
            new NativeArray<Vertex>(Count * 4 * 6, Allocator.Persistent);
        var indexData =
            new NativeArray<ushort>(Count * 4 * 6, Allocator.Persistent);

        // create job
        var job = new ChunkJob
        {
            position = Boundary.min,
            vertices = vertexData,
            indices = indexData
        };

        jobCompleter = new JobCompleter(() => job.Schedule(), () =>
        {
            // complete job and set mesh
            mesh = new Mesh
            {
                name = "node_mesh",
                bounds = Bounds
            };
            MeshingUtility.ApplyMesh(mesh, vertexData, indexData, Bounds);
        });
    }
}