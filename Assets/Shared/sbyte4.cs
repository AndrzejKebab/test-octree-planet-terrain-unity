using System;
using System.Runtime.CompilerServices;

#pragma warning disable 0659
[Serializable]
public struct sbyte4 : IEquatable<sbyte4>, IFormattable
{
    public sbyte x, y, z, w;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte4(sbyte x, sbyte y, sbyte z, sbyte w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(sbyte4 rhs) { return x == rhs.x && y == rhs.y && z == rhs.z && w == rhs.w; }

    public override bool Equals(object o) { return o is sbyte4 converted && Equals(converted); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return string.Format("sbyte4({0}, {1}, {2}, {3})", x, y, z, w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return string.Format("sbyte4({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
    }
}