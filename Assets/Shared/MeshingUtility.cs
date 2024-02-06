using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public static class MeshingUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ApplyMesh(Mesh meshData, NativeArray<Vertex> vertices, NativeArray<ushort> indices,
        Bounds bounds, IndexFormat indexFormat = IndexFormat.UInt16)
    {
        // Describe mesh data layout
        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp,
            NativeArrayOptions.UninitializedMemory
        )
        {
            [0] = new(VertexAttribute.Position, VertexAttributeFormat.Float16, 4),
            [1] = new(VertexAttribute.Normal, VertexAttributeFormat.Float16, 4),
            [2] = new(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
            [3] = new(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2)
        };

        meshData.SetVertexBufferParams(vertices.Length, vertexAttributes);
        meshData.SetVertexBufferData(vertices, 0, 0, vertices.Length);

        // Set Indices data
        meshData.SetIndexBufferParams(indices.Length, indexFormat);
        meshData.SetIndexBufferData(indices, 0, 0, indices.Length);

        // Set sub mesh
        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, indices.Length, MeshTopology.Quads)
        {
            bounds = bounds,
            vertexCount = vertices.Length,
        }, MeshUpdateFlags.DontRecalculateBounds);
    }
}