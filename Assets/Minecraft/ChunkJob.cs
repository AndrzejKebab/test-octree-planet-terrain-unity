using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct ChunkJob : IJob
{
    [ReadOnly] public float3 position;
    [WriteOnly] public NativeArray<Vertex> vertices;
    [WriteOnly] public NativeArray<ushort> indices;

    public void Execute()
    {
        const int indicesCount = Chunk.Count * 4 * 6; // 4 indices per quad * 6 quads
        const int vertexCount = Chunk.Count * 4 * 6; // 4 vertices per quad * 6 quads

        vertices = new NativeArray<Vertex>(vertexCount, Allocator.TempJob);
        indices = new NativeArray<ushort>(indicesCount, Allocator.TempJob);

        FastNoiseLite noise = new();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.01f);
        noise.SetSeed(2376);

        ushort vertexOffset = 0;

        for (var i = 0; i < Chunk.Count; i++)
        {
            var index = IndexUtilities.IndexToXyz(i, Chunk.Width, Chunk.Height);

            // if we are a solid block
            if (IsAir(index.x, index.y, index.z, in noise)) continue;

            // local mesh position of the voxel
            var pos = new half4((half)index.x, (half)index.y, (half)index.z, (half)0);

            for (var side = 0; side < 6; side++)
            {
                if (!IsAir(index.x + Tables.FaceChecks[side].x,
                        index.y + Tables.FaceChecks[side].y,
                        index.z + Tables.FaceChecks[side].z, in noise)) continue;
                
                NativeArray<half4> faceVertices = GetFaceVertices(side, pos);

                sbyte4 normal = new((sbyte)Tables.FaceChecks[side].x,
                    (sbyte)Tables.FaceChecks[side].y,
                    (sbyte)Tables.FaceChecks[side].z,
                    0);
                Color32 tangent = new((byte)normal.x, (byte)normal.y, (byte)normal.z, 0);

                vertices[vertexOffset + 0] = new Vertex(faceVertices[0], normal, tangent, new half2((half)0,(half)0));
                vertices[vertexOffset + 1] = new Vertex(faceVertices[1], normal, tangent, new half2((half)0,(half)1));
                vertices[vertexOffset + 2] = new Vertex(faceVertices[2], normal, tangent, new half2((half)1,(half)0));
                vertices[vertexOffset + 3] = new Vertex(faceVertices[3], normal, tangent, new half2((half)1,(half)1));

                // indices
                indices[vertexOffset + 0] = vertexOffset;
                indices[vertexOffset + 1] = (ushort)(vertexOffset + 1);
                indices[vertexOffset + 2] = (ushort)(vertexOffset + 2);
                indices[vertexOffset + 3] = (ushort)(vertexOffset + 3);

                // increment by 4 because we only added 4 vertices
                faceVertices.Dispose();
                vertexOffset += 4;
            }
        }

        //slice of valid data (otherwise mesh data is unnecessarily large)
        //var indicesSlice =
        //    new NativeArray<ushort>(vertexOffset, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //indices.Slice(0, vertexOffset).CopyTo(indicesSlice);
        //var verticesSlice =
        //    new NativeArray<Vertex>(vertexOffset, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //vertices.Slice(0, vertexOffset).CopyTo(verticesSlice);
    }

    private bool IsAir(int x, int y, int z, in FastNoiseLite noise)
    {
        // the voxels position in world coordinates
        var worldVoxelPosition = new float3(x, y, z) + position;

        var height = ((noise.GetNoise(worldVoxelPosition.x, worldVoxelPosition.z) + 1f) / 2f) * (Chunk.Height / 2);
        return worldVoxelPosition.y > height;
    }

    private static NativeArray<half4> GetFaceVertices(int faceIndex, half4 pos)
    {
        var faceVertices = new NativeArray<half4>(4, Allocator.TempJob);

        for (byte i = 0; i < 4; i++)
        {
            var index = Tables.VoxelTriangles[(faceIndex * 4) + i];
            faceVertices[i] = new half4((half)(Tables.Vertices[index].x + pos.x),
                (half)(Tables.Vertices[index].y + pos.y),
                (half)(Tables.Vertices[index].z + pos.z),
                (half)0);
        }

        return faceVertices;
    }
}