using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.ProceduralTerrain;
using UnityEditorInternal;
using Assets.ProceduralTerrain.Utils;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using Random = UnityEngine.Random;

public class TerrainChunk : MonoBehaviour
{
    [Header("Chunk")]
    public int HeightmapResolution;
    public float ChunkLength;
    public float ChunkHeight;

    [Header("Objects")]
    public int TreesMaxCount;
    public int BushesMaxCount;
    public int RocksMaxCount;

    [Header("Terrain")]
    public Material Material;
    public int DetailResolution;
    public float DetailObjectDensity;
    public float DetailObjectDistance;

    [Header("Grass")]
    public float WavingGrassStrength;
    public float WavingGrassAmount;
    public float WavingGrassSpeed;
    public List<GrassDetails> GrassPrototypes;

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    private GameObjectPool _treePool;
    private GameObjectPool _bushesPool;
    private GameObjectPool _rockPool;
    private GameObjectManager _treeManager;
    private GameObjectManager _bushManager;
    private GameObjectManager _rockManager;

    private TerrainData _terrainData;
    private Terrain _terrain;
    private float[,] _heightmap;
    private List<int[,]> _detailmaps;

    private System.Random _random;

    public void Awake()
    {
        _treePool = GameObject.FindGameObjectWithTag("TreePool").GetComponent<GameObjectPool>();
        _bushesPool = GameObject.FindGameObjectWithTag("BushesPool").GetComponent<GameObjectPool>();
        _rockPool = GameObject.FindGameObjectWithTag("RockPool").GetComponent<GameObjectPool>();
        _treeManager = new GameObjectManager(_treePool, RandomPositions(TreesMaxCount));
        _bushManager = new GameObjectManager(_bushesPool, RandomPositions(BushesMaxCount));
        _rockManager = new GameObjectManager(_rockPool, RandomRockPositions());

        _heightmap = new float[HeightmapResolution, HeightmapResolution];
        _detailmaps = new List<int[,]>();
        _random = new System.Random();

        CreateHeightmap();
        CreateDetailmap();
        CreateTerrainData();
        _terrain = Terrain.CreateTerrainGameObject(_terrainData).GetComponent<Terrain>();
        InitTextures();
        InitGrass();

    }


    void InitGrass()
    {
        var detailPrototypes = GrassPrototypes.Select(grassPrototype =>
            new DetailPrototype
            {
                prototypeTexture = grassPrototype.GrassTexture,
                healthyColor = grassPrototype.HealthyColor,
                dryColor = grassPrototype.DryColor,
                renderMode = DetailRenderMode.Grass
            });
        _terrainData.detailPrototypes = detailPrototypes.ToArray();
        _terrainData.wavingGrassStrength = WavingGrassStrength;
        _terrainData.wavingGrassAmount = WavingGrassAmount;
        _terrainData.wavingGrassSpeed = WavingGrassSpeed;
        _terrain.detailObjectDensity = DetailObjectDensity;
        _terrain.detailObjectDistance = DetailObjectDistance;
        _terrainData.SetDetailResolution(DetailResolution, DetailResolution / 2);
        for (int i = 0; i < GrassPrototypes.Count; i++)
        {
            _terrainData.SetDetailLayer(0, 0, i, _detailmaps[i]);
        }
        
    }

    public void OnEnable()
    {
        Activate();
    }

    public void OnDisable()
    {
        Deactivate();
    }

    //////////////////////// INIT/////////////////////////

    private void CreateTerrainData()
    {
        _terrainData = new TerrainData();
        _terrainData.heightmapResolution = HeightmapResolution;
        _terrainData.alphamapResolution = HeightmapResolution;
        _terrainData.size = new Vector3(ChunkLength, ChunkHeight, ChunkLength);
        _terrainData.SetHeights(0, 0, _heightmap);
    }

    private void InitTextures()
    {
        _terrain.materialTemplate = Material;
        _terrain.materialType = Terrain.MaterialType.Custom;
    }

    /////////////////////// ACTIVATE ///////////////////////

    public void Activate()
    {
        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        yield return StartCoroutine(ActivateTerrain());
        yield return StartCoroutine(_treeManager.Activate());
        yield return StartCoroutine(_bushManager.Activate());
        yield return StartCoroutine(_rockManager.Activate());
        FadeIn();
    }

    private IEnumerator ActivateTerrain()
    {
        _terrain.gameObject.transform.position = new Vector3(ChunkX * ChunkLength, 0, ChunkZ * ChunkLength);
        CreateHeightmap();
        _terrainData.SetHeights(0, 0, _heightmap);
        _terrain.gameObject.SetActive(true);
        yield return null;
    }

    /////////////////////// DEACTIVATE ///////////////////////

    private void Deactivate()
    {
        DeactivateTerrain();
        _treeManager.Deactivate();
        _bushManager.Deactivate();
        _rockManager.Deactivate();
    }

    private void DeactivateTerrain()
    {
        if (_terrain != null)
        {
            _terrain.gameObject.SetActive(false);
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeUtils.FadeIn(GetObjectsToFade()));
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeUtils.FadeOut(GetObjectsToFade()));
    }

    private List<GameObject> GetObjectsToFade()
    {
        List<GameObject> objectsToFade = new List<GameObject>();
        objectsToFade.AddRange(_treeManager.ObjectList);
        objectsToFade.AddRange(_bushManager.ObjectList);
        objectsToFade.AddRange(_rockManager.ObjectList);
        return objectsToFade;
    }

    /////////////////////////////// OTHER ///////////////////////////////

    private void CreateHeightmap()
    {
        for (var zRes = 0; zRes < HeightmapResolution; zRes++)
        {
            for (var xRes = 0; xRes < HeightmapResolution; xRes++)
            {
                var xCoordinate = ChunkX + xRes / (HeightmapResolution - 1f);
                var zCoordinate = ChunkZ + zRes / (HeightmapResolution - 1f);
                _heightmap[zRes, xRes] = Mathf.PerlinNoise(xCoordinate, zCoordinate);
            }
        }
    }

    private void CreateDetailmap()
    {
        for (int i = 0; i < GrassPrototypes.Count; i++)
        {
            _detailmaps.Insert(i, new int[DetailResolution, DetailResolution]);
            for (var zRes = 0; zRes < DetailResolution; zRes++)
            {
                for (var xRes = 0; xRes < DetailResolution; xRes++)
                {
                    _detailmaps[i][zRes, xRes] = _random.NextDouble() < GrassPrototypes[i].Probability ? 1 : 0;
                }
            }
        }
    }

    private GetPositionsDelegate RandomRockPositions()
    {
        return delegate
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < _random.Next(RocksMaxCount); i++)
            {
                positions.Add(RandomTreePosition() + new Vector3(0, 0.2f, 0));
            }
            return positions;
        };
    }

    private GetPositionsDelegate RandomPositions(int maxCount)
    {
        return delegate
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < _random.Next(maxCount); i++)
            {
                positions.Add(RandomTreePosition());
            }
            return positions;
        };
    }

    private Vector3 RandomTreePosition()
    {
        var terrainPosition = _terrain.GetPosition();
        var treeRelativeX = Random.value % ChunkLength;
        var treeRelativeZ = Random.value % ChunkLength;
        var xRes = ((ChunkX + treeRelativeX) * HeightmapResolution);
        var zRes = ((ChunkZ + treeRelativeZ) * HeightmapResolution);

        var treeX = terrainPosition.x + treeRelativeX * ChunkLength;
        var treeY = _terrainData.GetHeight((int)xRes, (int)zRes) - 0.5;
        var treeZ = terrainPosition.z + treeRelativeZ * ChunkLength;
        return new Vector3(treeX, (float)treeY, treeZ);
    }

}