using Unity.Mathematics;
using UnityEngine;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct Vertex
{
    public half4 Position;
    public sbyte4 Normal;
    public Color32 Color;
    public half2 UVs;

    public Vertex(half4 position, sbyte4 normal, Color32 color, half2 uv)
    {
        Position = position;
        Normal = normal;
        Color = color;
        UVs = uv;
    }
}