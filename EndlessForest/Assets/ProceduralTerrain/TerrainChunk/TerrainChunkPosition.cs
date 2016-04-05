using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TerrainChunkPosition : IEquatable<TerrainChunkPosition>
{
    public int x { get; private set; }
    public int z { get; private set; }

    public TerrainChunkPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(TerrainChunkPosition other)
    {
        return other.x == x && other.z == z;
    }

    public override int GetHashCode()
    {
        return (x + "-" + z).GetHashCode();
    }

    public override string ToString()
    {
        return "(" + x + "," + z + ")";
    }
}
