using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public struct ChunkJob : IJob
{
    public Bounds bounds;
    public Mesh.MeshDataArray meshDataArray;
    public Vector3 planetCenterPosition;
    public float planetRadius;

    public int chunkResolution;
    public float nodeScale;
    public float3 worldNodePosition;
    public float voxelScale;

    private float3 centerOffset;
    private float normalizedVoxelScale;


    public void Execute()
    {
        int meshCubeCount = (int)math.ceil((chunkResolution * chunkResolution * chunkResolution) / 2);
        int indiceCount = meshCubeCount * 6 * 6; // 6 indicies per quad * 6 quads
        int vertexCount = meshCubeCount * 4 * 6; // 4 vertices per quad * 6 quads

        var indices = new NativeArray<uint>(indiceCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var vertices = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var normals = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        // so we can offset the mesh to the the center of the gameObject
        centerOffset = Vector3.one * (nodeScale / 2);

        // nodeScale divided by the the number of voxels in the node
        normalizedVoxelScale = nodeScale / chunkResolution;

        FastNoiseLite noise = new FastNoiseLite();

        // variables
        int vertexOffset = 0;
        int indiceOffset = 0;

        for (int x = 0; x < chunkResolution; x++)
        {
            for (int y = 0; y < chunkResolution; y++)
            {
                for (int z = 0; z < chunkResolution; z++)
                {
                    if (IsSolid(x, y, z, noise))
                    {
                        // local mesh position of the voxel
                        float3 pos = (new float3(x, y, z) * normalizedVoxelScale) - centerOffset;

                        for (int side = 0; side < 6; side++)
                        {
                            if (!IsSolid(x + Tables.NeighborOffset[side].x, y + Tables.NeighborOffset[side].y, z + Tables.NeighborOffset[side].z, noise))
                            {
                                // vertices
                                vertices[vertexOffset + 0] = Tables.Vertices[Tables.BuildOrder[side, 0]] * normalizedVoxelScale + pos;
                                vertices[vertexOffset + 1] = Tables.Vertices[Tables.BuildOrder[side, 1]] * normalizedVoxelScale + pos;
                                vertices[vertexOffset + 2] = Tables.Vertices[Tables.BuildOrder[side, 2]] * normalizedVoxelScale + pos;
                                vertices[vertexOffset + 3] = Tables.Vertices[Tables.BuildOrder[side, 3]] * normalizedVoxelScale + pos;

                                // normals
                                normals[vertexOffset + 0] = Tables.Normals[side];
                                normals[vertexOffset + 1] = Tables.Normals[side];
                                normals[vertexOffset + 2] = Tables.Normals[side];
                                normals[vertexOffset + 3] = Tables.Normals[side];

                                // indices
                                indices[indiceOffset + 0] = (uint)vertexOffset + 0;
                                indices[indiceOffset + 1] = (uint)vertexOffset + 1;
                                indices[indiceOffset + 2] = (uint)vertexOffset + 2;
                                indices[indiceOffset + 3] = (uint)vertexOffset + 2;
                                indices[indiceOffset + 4] = (uint)vertexOffset + 1;
                                indices[indiceOffset + 5] = (uint)vertexOffset + 3;

                                // increment by 4 because we only added 4 vertices
                                vertexOffset += 4;

                                // increment by 6 because we added 6 int's to our triangles array
                                indiceOffset += 6;
                            }
                        }
                    }
                }
            }
        }

        // Slice of valid data (otherwise mesh data is uneccessarily large)
        var indicesSlice = new NativeArray<uint>(indiceOffset, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        indices.Slice(0, (int)indiceOffset).CopyTo(indicesSlice);
        var verticesSlice = new NativeArray<float3>(vertexOffset, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        vertices.Slice(0, (int)vertexOffset).CopyTo(verticesSlice);
        var normalsSlice = new NativeArray<float3>(vertexOffset, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        normals.Slice(0, (int)vertexOffset).CopyTo(normalsSlice);

        // apply the mesh to the meshDataArray
        MeshingUtility.ApplyMesh(ref meshDataArray, ref indicesSlice, ref verticesSlice, ref normalsSlice, ref bounds);
    }
    /// <summary>
    /// Checks if a voxel is solid
    /// </summary>
    /// <param name="x">local voxel index</param>
    /// <param name="y">local voxel index</param>
    /// <param name="z">local voxel index</param>
    /// <param name="noise"></param>
    /// <returns></returns>
    private bool IsSolid(int x, int y, int z, FastNoiseLite noise)
    {
        // make outer voxels solid
        if (x < 0 || x > chunkResolution - 1 ||
            y < 0 || y > chunkResolution - 1 ||
            z < 0 || z > chunkResolution - 1)
        {
            return false;
        }

        // the voxels position in world coordinates
        float3 worldVoxelPosition = ((new float3(x, y, z) * normalizedVoxelScale) + worldNodePosition) - centerOffset;

        // a noise for distorting the surface of the planet
        float planetSurfaceDistortion = noise.GetNoise(worldVoxelPosition.x, worldVoxelPosition.y, worldVoxelPosition.z) * 20f;

        // make a sphere planet!
        float distance = Vector3.Distance(worldVoxelPosition, planetCenterPosition) + planetSurfaceDistortion;

        // above ground
        if (distance > planetRadius)
        {
            return false; // air
        }
        // below ground
        else
        {
            return true; // ground
        }
    }
}
