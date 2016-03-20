using UnityEngine;
using System.Collections;
using LibNoise.Unity.Generator;
using Wasabimole.ProceduralTree;

public class TerrainChunk
{
    public Terrain Terrain { get; private set; }
    public TerrainCollider TerrainCollider { get; private set; }
    public Texture2D Texture { get; private set; }    

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }
    public int HeightmapResolution { get; set; }
    public float ChunkLength { get; set; }
    public float ChunkHeight { get; set; }
    public Material Material { get; set; }
    public ProceduralTree TreePrefab { get; set; }

    public void Render()
    {
        var terrainData = CreateTerrainData();
        var terrainGameObject = CreateTerrain(terrainData);
        Terrain = terrainGameObject.GetComponent<Terrain>();
        TerrainCollider = terrainGameObject.GetComponent<TerrainCollider>();
        ApplyTextures(Terrain);
        RenderTrees(Terrain);
        Terrain.Flush();
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

    private GameObject CreateTerrain(TerrainData terrainData)
    {
        var terrainGameObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainGameObject.transform.position = new Vector3(ChunkX * ChunkLength, 0, ChunkZ * ChunkLength);
        return terrainGameObject;
    }

    private float[,] CreateHeightmap()
    {
        var heightmap = new float[HeightmapResolution, HeightmapResolution];
        for (var zRes = 0; zRes < HeightmapResolution; zRes++)
        {
            for (var xRes = 0; xRes < HeightmapResolution; xRes++)
            {
                var xCoordinate = ChunkX + ((float)xRes /( HeightmapResolution - 1f));
                var zCoordinate = ChunkZ + ((float)zRes /( HeightmapResolution - 1f));
                heightmap[zRes, xRes] = Mathf.PerlinNoise(xCoordinate, zCoordinate);
            }
        }
        return heightmap;
    }

    private void ApplyTextures(Terrain terrain)
    {
        Texture = new Texture2D((int)ChunkLength, (int)ChunkLength);
        Material.SetTexture(1, Texture);
        Terrain.materialTemplate = Material;
        Terrain.materialType = Terrain.MaterialType.Custom;
    }

    private void RenderTrees(Terrain terrain)
    {
        var xRes = ((ChunkX) * HeightmapResolution + 1);
        var zRes = ((ChunkZ) * HeightmapResolution + 1);
        var pos = Terrain.GetPosition();
        var position = new Vector3(pos.x, terrain.terrainData.GetHeight((int)xRes, (int)zRes), pos.z);
        TreePrefab.Seed = Random.seed;
        var tree = GameObject.Instantiate(TreePrefab, position, Quaternion.identity);
        tree.name = Random.value.ToString();
    }
}
