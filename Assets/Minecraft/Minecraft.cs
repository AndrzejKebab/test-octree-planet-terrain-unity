using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Minecraft : MonoBehaviour
{
    [field:SerializeField] public Material Material { get; private set; }
    [SerializeField] private int distance = 4;
    [SerializeField] private new Camera camera;

    private readonly Dictionary<int3, Chunk> chunks = new();
    public readonly List<JobCompleter> ToComplete = new();

    public Plane[] Frustrum;

    private void Start()
    {
        Frustrum = GeometryUtility.CalculateFrustumPlanes(camera);
        for (var x = 0; x < distance; x++)
        {
            for (var z = 0; z < distance; z++)
            {
                int3 key = new(x, 0, z);
                var chunk = new Chunk(this, key);
                chunks.Add(key, chunk);
            }
        }
    }

    private void Update()
    {
        Frustrum = GeometryUtility.CalculateFrustumPlanes(camera);
        if (ToComplete.Count > 0)
        {
            var timer = new Stopwatch();
            timer.Start();
            NativeArray<JobHandle> jobHandles = new(ToComplete.Count, Allocator.Temp);
            for (var i = 0; i < ToComplete.Count; i++)
            {
                jobHandles[i] = ToComplete[i].Schedule();
            }

            JobHandle.CompleteAll(jobHandles);
            jobHandles.Dispose();

            for (var i = 0; i < ToComplete.Count; i++)
            {
                ToComplete[i].OnComplete();
            }

            ToComplete.Clear();
            timer.Stop();
            UnityEngine.Debug.Log(timer.ElapsedMilliseconds + "ms");
        }

        foreach (var chunk in chunks.Values)
        {
            if (GeometryUtility.TestPlanesAABB(Frustrum, chunk.Boundary))
            {
                chunk.Draw();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (chunks.Count <= 0) return;
        foreach (var chunk in chunks.Values)
        {
            Gizmos.color = GeometryUtility.TestPlanesAABB(Frustrum, chunk.Boundary) ? Color.blue : Color.red;

            Gizmos.DrawWireCube(chunk.Boundary.center, chunk.Boundary.size);
        }
    }
}