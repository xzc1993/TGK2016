using System;

class TerrainChunkPosition : IEquatable<TerrainChunkPosition>
{
    public int X { get; private set; }
    public int Z { get; private set; }

    public TerrainChunkPosition(int x, int z)
    {
        X = x;
        Z = z;
    }

    public bool Equals(TerrainChunkPosition other)
    {
        return other.X == X && other.Z == Z;
    }

    public override int GetHashCode()
    {
        return (X + "-" + Z).GetHashCode();
    }

    public override string ToString()
    {
        return "(" + X + "," + Z + ")";
    }
}
