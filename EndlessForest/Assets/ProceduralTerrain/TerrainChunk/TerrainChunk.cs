using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wasabimole.ProceduralTree;

public class TerrainChunk
{
    public Terrain Terrain { get; private set; }
    public TerrainData TerrainData { get; private set; }
    public TerrainCollider TerrainCollider { get; private set; }
    public Texture2D Texture { get; private set; }
    public List<ProceduralTree> ActiveTrees = new List<ProceduralTree>(); 

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }
    public int HeightmapResolution { get; set; }
    public float ChunkLength { get; set; }
    public float ChunkHeight { get; set; }
    public Material Material { get; set; }
    public ProceduralTree TreePrefab { get; set; }
    public int TreesCount { get; set; }
    private float[,] heightmap;

    private TerrainChunkElementsPool _elementsPool;

    public bool Activated { get; private set; }


    public TerrainChunk()
    {
        Activated = false;
    }

    public void Init()
    {
        heightmap = new float[HeightmapResolution, HeightmapResolution];
        TerrainData = new TerrainData();
        var terrainGameObject = Terrain.CreateTerrainGameObject(TerrainData);
        Terrain = terrainGameObject.GetComponent<Terrain>();
        TerrainCollider = terrainGameObject.GetComponent<TerrainCollider>();
        _elementsPool = new TerrainChunkElementsPool(TreesCount + 5, TreePrefab);
        InitTextures();
    }

    public void Deactivate()
    {
        DeactivateTrees();
        Terrain.gameObject.SetActive(false);
        Activated = false;
    }

    public void Activate()
    {
        if (Terrain == null)
        {
            Init();
        }
        ActivateTerrain();
        //ActivateTrees();
        ApplyTerrainTextures();
        Terrain.Flush();
        Terrain.gameObject.SetActive(true);
        
    }

    public void ActivateTrees()
    {
        if (ActiveTrees.Count < TreesCount)
        {
            Vector3 treePosition = RandomTreePosition();
            ProceduralTree tree = _elementsPool.takeTree();
            tree.transform.position = treePosition;
            tree.gameObject.SetActive(true);
            ActiveTrees.Add(tree);
        }
        else
        {
            Activated = true;
        }
        
    }

    private void DeactivateTrees()
    {
        foreach (var tree in ActiveTrees)
        {
            tree.gameObject.SetActive(false);
            _elementsPool.putTree(tree);
        }
    }

    private void ActivateTerrain()
    {
        Terrain.gameObject.transform.position = new Vector3(ChunkX * ChunkLength, 0, ChunkZ * ChunkLength);
        
        TerrainData = CreateTerrainData();
        Terrain.terrainData = TerrainData;
        TerrainCollider.terrainData = TerrainData;
    }

    private TerrainData CreateTerrainData()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = HeightmapResolution;
        terrainData.alphamapResolution = HeightmapResolution;
        terrainData.size = new Vector3(ChunkLength, ChunkHeight, ChunkLength);
        terrainData.SetHeights(0, 0, CreateHeightmap());
        return terrainData;
    }

    private float[,] CreateHeightmap()
    {
        Profiler.BeginSample("CreateHeightmap");
        
        
        for (var zRes = 0; zRes < HeightmapResolution; zRes++)
        {
            for (var xRes = 0; xRes < HeightmapResolution; xRes++)
            {
                var xCoordinate = ChunkX + ((float)xRes /( HeightmapResolution - 1f));
                var zCoordinate = ChunkZ + ((float)zRes /( HeightmapResolution - 1f));
                heightmap[zRes, xRes] = Mathf.PerlinNoise(xCoordinate, zCoordinate);
            }
        }
        Profiler.EndSample();
        return heightmap;
    }

    private void InitTextures()
    {
        Texture = new Texture2D((int)ChunkLength, (int)ChunkLength);
        Material.SetTexture(1, Texture);
        Terrain.materialTemplate = Material;
        Terrain.materialType = Terrain.MaterialType.Custom;
    }

    private void ApplyTerrainTextures()
    {

    }

//    private List<Vector3> RandomTreePositions()
//    {
//        List<Vector3> positions = new List<Vector3>(TreesCount);
//        for (int i = 0; i < TreesCount; i++)
//        {
//
//            positions.Add(treePosition);
//        }
//        return positions;
//    }

    private Vector3 RandomTreePosition()
    {
        var terrainPosition = Terrain.GetPosition();
        var treeRelativeX = Random.value % ChunkLength;
        var treeRelativeZ = Random.value % ChunkLength;
        var xRes = ((ChunkX + treeRelativeX) * HeightmapResolution);
        var zRes = ((ChunkZ + treeRelativeZ) * HeightmapResolution);

        var treeX = terrainPosition.x + treeRelativeX * ChunkLength;
        var treeY = Terrain.terrainData.GetHeight((int)xRes, (int)zRes) - 0.1;
        var treeZ = terrainPosition.z + treeRelativeZ * ChunkLength;
        return new Vector3(treeX, (float)treeY, treeZ);
    }

}
