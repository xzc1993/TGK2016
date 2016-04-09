using UnityEngine;
using Wasabimole.ProceduralTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralTerrain;

public class TerrainManager : MonoBehaviour
{
    public TerrainChunk TerrainChunkPrefab;
    public int NeighbourhoodSize;

    private Dictionary<TerrainChunkPosition, TerrainChunk> _map;
    private GameObjectPool _terrainChunkPool;
    private GameObject _playerGameObject;

    void Start ()
    {
        Profiler.enabled = true;
        _map = new Dictionary<TerrainChunkPosition, TerrainChunk>();
        _terrainChunkPool = GameObject.FindGameObjectWithTag("TerrainChunkPool").GetComponent<GameObjectPool>();
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update ()
    {
        StartCoroutine(DeactivateFarAwayTerrain());
        ActivateNeighborhoodTerrain();
    }

    private void ActivateNeighborhoodTerrain()
    {
        TerrainChunkPosition playerPosition = GetPlayerChunkPosition();
        var minX = playerPosition.X - NeighbourhoodSize;
        var maxX = playerPosition.X + NeighbourhoodSize;
        var minZ = playerPosition.Z - NeighbourhoodSize;
        var maxZ = playerPosition.Z + NeighbourhoodSize;

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                var chunkPosition = new TerrainChunkPosition(x, z);
                if (!_map.ContainsKey(chunkPosition))
                {
                    var terrainChunk = _terrainChunkPool.Take<TerrainChunk>();
                    terrainChunk.ChunkX = x;
                    terrainChunk.ChunkZ = z;
                    _map[chunkPosition] = terrainChunk;
                    terrainChunk.gameObject.SetActive(true);
                }
            }
        }
    }

    private IEnumerator DeactivateFarAwayTerrain()
    {
        TerrainChunkPosition playerPosition = GetPlayerChunkPosition();
        var minX = playerPosition.X - NeighbourhoodSize;
        var maxX = playerPosition.X + NeighbourhoodSize;
        var minZ = playerPosition.Z - NeighbourhoodSize;
        var maxZ = playerPosition.Z + NeighbourhoodSize;

        var farAwayChunkPositions = _map.Keys.Where(key => key.X < minX || key.X > maxX || key.Z < minZ || key.Z > maxZ).ToList();
        foreach (var position in farAwayChunkPositions)
        {
            if (_map.ContainsKey(position))
            {
                TerrainChunk terrainChunk = _map[position];
                _map.Remove(position);
                yield return StartCoroutine(terrainChunk.FadeOut());
                terrainChunk.gameObject.SetActive(false);
                _terrainChunkPool.Put(terrainChunk.gameObject);
            }
        }
    }

    private TerrainChunkPosition GetPlayerChunkPosition()
    {
        Vector3 position = _playerGameObject.transform.position;
        int chunkX = (int) (position.x/ TerrainChunkPrefab.ChunkLength);
        int chunkZ = (int) (position.z/ TerrainChunkPrefab.ChunkLength);
        return new TerrainChunkPosition(chunkX, chunkZ);
    }
}
