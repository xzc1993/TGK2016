using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wasabimole.ProceduralTree;

public class TerrainChunkPool
{
    // Just for creating new Terrain chunks
    public Material Material { get; set; }
    public ProceduralTree TreePrefab { get; set;}
    public int HeightmapResolution { get; set; }
    public float ChunkLength { get; set; }
    public float ChunkHeight { get; set; }
    public int NeighbourhoodSize { get; set; }
    public int PoolSize { get; set; }
    public int TreesCount { get; set; }

    private readonly Queue<TerrainChunk> _inactiveTerrainChunks;

    public TerrainChunkPool(int poolSize)
    {
        PoolSize = poolSize;
        _inactiveTerrainChunks = new Queue<TerrainChunk>(PoolSize);
    }

    public void Init()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            TerrainChunk terrainChunk = CreateTerrainChunk();
            this.put(terrainChunk);
        }
    }

    public void put(TerrainChunk terrainChunk)
    {
//        if (terrainChunk.Activated)
//        {
//            terrainChunk.Deactivate();
//        }
        _inactiveTerrainChunks.Enqueue(terrainChunk);
    }

    public TerrainChunk take()
    {
        if (_inactiveTerrainChunks.Count == 0)
        {
            TerrainChunk terrainChunk = CreateTerrainChunk();
            put(terrainChunk);
        }
       return  _inactiveTerrainChunks.Dequeue();
    }

    private TerrainChunk CreateTerrainChunk()
    {
        TerrainChunk terrainChunk = new TerrainChunk
        {
            HeightmapResolution = HeightmapResolution,
            ChunkHeight = ChunkHeight,
            ChunkLength = ChunkLength,
            Material = Material,
            TreePrefab = TreePrefab,
            TreesCount = TreesCount
        };
        terrainChunk.Init();
        return terrainChunk;
    }
}
