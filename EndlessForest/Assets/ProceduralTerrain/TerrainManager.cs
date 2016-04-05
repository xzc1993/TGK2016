using UnityEngine;
using Wasabimole.ProceduralTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TerrainManager : MonoBehaviour
{
    // Parameters for Unity
    public Material Material;
    [Range(5, 10)]
    public ProceduralTree ProceduralTreePrefab;
    public int HeightmapResolution;
    public float ChunkLength;
    public float ChunkHeight;
    public int NeighbourhoodSize;
    public int TreesCount;

    private Dictionary<TerrainChunkPosition, TerrainChunk> _map = new Dictionary<TerrainChunkPosition, TerrainChunk>();
    private TerrainChunkPool terrainChunkPool;
    private GameObject _playerGameObject;
    

    // Use this for initialization
    void Start ()
    {
        ProceduralTreePrefab.MaxNumVertices = 5000;
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        terrainChunkPool = new TerrainChunkPool((int) Math.Pow(NeighbourhoodSize * 2 + 1, 2))
        {
            HeightmapResolution = (int) Mathf.Pow(2, HeightmapResolution) + 1,
            ChunkHeight = ChunkHeight,
            ChunkLength = ChunkLength,
            Material = Material,
            TreePrefab = ProceduralTreePrefab,
            TreesCount = TreesCount
        };
        terrainChunkPool.Init();
        DeactivateFarAwayTerrain(true);
        ActivateNeighborhoodTerrain(true);
        Profiler.enabled = true;
        Profiler.maxNumberOfSamplesPerFrame = 100000;
	}

    // Update is called once per frame
    void Update ()
    {
        DeactivateFarAwayTerrain(false);
        ActivateNeighborhoodTerrain(false);
    }

    private void ActivateNeighborhoodTerrain(bool firstExecute)
    {
        TerrainChunkPosition playerPosition = getPlayerChunkPosition();
        var minX = playerPosition.x - NeighbourhoodSize;
        var maxX = playerPosition.x + NeighbourhoodSize;
        var minZ = playerPosition.z - NeighbourhoodSize;
        var maxZ = playerPosition.z + NeighbourhoodSize;

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                var chunkPosition = new TerrainChunkPosition(x, z);
                if (!_map.ContainsKey(chunkPosition))
                {
                    var terrainChunk = terrainChunkPool.take();
                    terrainChunk.ChunkX = x;
                    terrainChunk.ChunkZ = z;
                    terrainChunk.Activate();
                    terrainChunk.ActivateTrees();
                    _map[chunkPosition] = terrainChunk;
                    if (!firstExecute)
                    {
                        break;
                    }
                }
                /*else
                {
                    var terrainChunk = _map[chunkPosition];
                    if (!terrainChunk.Activated)
                    {
                        
                    }
                }*/
            }
        }
    }

    private void DeactivateFarAwayTerrain(bool firstExecute)
    {
        TerrainChunkPosition playerPosition = getPlayerChunkPosition();
        var minX = playerPosition.x - NeighbourhoodSize;
        var maxX = playerPosition.x + NeighbourhoodSize;
        var minZ = playerPosition.z - NeighbourhoodSize;
        var maxZ = playerPosition.z + NeighbourhoodSize;

        var farAwayChunkPositions = _map.Keys.Where(key => key.x < minX || key.x > maxX || key.z < minZ || key.z > maxZ).ToList();
        foreach (var position in farAwayChunkPositions)
        {
            var terrainChunk = _map[position];
            if (terrainChunk != null)
            {
                _map[position] = null;
                terrainChunk.Deactivate();
                terrainChunkPool.put(terrainChunk);
                if (!firstExecute)
                {
                    break;
                }
            }
        }
    }

    private TerrainChunkPosition getPlayerChunkPosition()
    {
        Vector3 position = _playerGameObject.transform.position;
        int chunkX = (int) (position.x/ChunkLength);
        int chunkZ = (int) (position.z/ChunkLength);
        return new TerrainChunkPosition(chunkX, chunkZ);
    }
}
