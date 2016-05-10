using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using Assets.ProceduralTerrain;
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
	public int AnimalMaxCount;

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

    [Header("Lakes")]
    public GameObject WaterPrefab;
    public float WaterProbability;
    public float SeaLevel;

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    private GameObjectPool _treePool;
    private GameObjectPool _bushesPool;
    private GameObjectPool _rockPool;
    private GameObjectPool _waterPool;
	private GameObjectPool _velociraptorPool;
    private GameObjectManager _treeManager;
    private GameObjectManager _bushManager;
    private GameObjectManager _rockManager;
    private GameObjectManager _waterManager;
	public GameObjectManager _velociraptorManager;

    private TerrainData _terrainData;
    private Terrain _terrain;
    private float[,] _heightmap;
    private List<int[,]> _detailmaps;

    private System.Random _random;


    private bool _activated;
    private bool _activating;

    public void Awake()
    {
        _treePool = GameObject.FindGameObjectWithTag("TreePool").GetComponent<GameObjectPool>();
        _bushesPool = GameObject.FindGameObjectWithTag("BushesPool").GetComponent<GameObjectPool>();
        _rockPool = GameObject.FindGameObjectWithTag("RockPool").GetComponent<GameObjectPool>();
        _waterPool = GameObject.FindGameObjectWithTag("WaterPool").GetComponent<GameObjectPool>();
		_velociraptorPool = GameObject.FindGameObjectWithTag("VelociraptorPool").GetComponent<GameObjectPool>();

        _treeManager = new GameObjectManager(_treePool, RandomPositions(TreesMaxCount));
        _bushManager = new GameObjectManager(_bushesPool, RandomPositions(BushesMaxCount));
        _rockManager = new GameObjectManager(_rockPool, RandomRockPositions());
        _waterManager = new GameObjectManager(_waterPool, RandomLakePositions(), false);
		_velociraptorManager = new GameObjectManager(_velociraptorPool, RandomAnimalPositions(), false);

        _heightmap = new float[HeightmapResolution, HeightmapResolution];
        _detailmaps = new List<int[,]>();
        _random = new System.Random();

    }

    private IEnumerator Init()
    {
        _activating = true;
        CreateTerrainData();
		GameObject terrainObject = Terrain.CreateTerrainGameObject(_terrainData);
		terrainObject.AddComponent<TerrainChunkAnimalTracker>();
		terrainObject.GetComponent<TerrainChunkAnimalTracker>().setHomeChunk( this);
		_terrain = terrainObject.GetComponent<Terrain>();
        InitTextures();
        CreateDetailmap();
        InitGrass();
        _activating = false;
        _activated = true;
        yield return null;
    }

    public void OnEnable()
    {
        Activate();
    }

    public void OnDisable()
    {
        Deactivate();
    }

	public GameObjectManager getVelociraptorManager(){
		return this._velociraptorManager;
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

    /////////////////////// ACTIVATE ///////////////////////

    public void Activate()
    {
        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        if (!_activating && !_activated)
        {
            yield return StartCoroutine(Init());
        }
        yield return StartCoroutine(ActivateTerrain());
        yield return StartCoroutine(_treeManager.Activate());
        yield return StartCoroutine(_bushManager.Activate());
        yield return StartCoroutine(_rockManager.Activate());
        yield return StartCoroutine(_waterManager.Activate());
		yield return StartCoroutine(_velociraptorManager.Activate());
        FadeIn();
    }

    private IEnumerator ActivateTerrain()
    {
        _terrain.gameObject.transform.position = new Vector3(ChunkX * ChunkLength, 0, ChunkZ * ChunkLength);
        CreateHeightmap();
        CreateDetailmap();
        _terrainData.SetHeights(0, 0, _heightmap);
        for (int i = 0; i < GrassPrototypes.Count; i++)
        {
            _terrainData.SetDetailLayer(0, 0, i, _detailmaps[i]);
        }
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
        _waterManager.Deactivate();
		_velociraptorManager.Deactivate();
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
        //objectsToFade.AddRange(_waterManager.ObjectList);
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
                _heightmap[zRes, xRes] =  Mathf.PerlinNoise(xCoordinate, zCoordinate);
                _heightmap[zRes, xRes] -= 0.5f * Mathf.PerlinNoise(1000 + xCoordinate, 1000 + zCoordinate);
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

    
    private GetPositionsDelegate RandomLakePositions()
    {
        return delegate
        {
            List<Vector3> positions = new List<Vector3>();
            for (var zRes = 0; zRes < HeightmapResolution; zRes += 6)
            {
                for (var xRes = 0; xRes < HeightmapResolution; xRes += 6)
                {
                    var xCoordinate = ChunkX + xRes / (HeightmapResolution - 1f);
                    var zCoordinate = ChunkZ + zRes / (HeightmapResolution - 1f);
                    if (_heightmap[zRes, xRes] < (SeaLevel / ChunkHeight))
                    {
                        positions.Add(new Vector3(xCoordinate * ChunkLength, SeaLevel, zCoordinate * ChunkLength));
                    }
                }
            }
            return positions;
        };
    }

	private GetPositionsDelegate RandomAnimalPositions()
	{
		return RandomPositions(AnimalMaxCount);
	}

	private GetPositionsDelegate RandomRockPositions()
	{
		return RandomPositions(RocksMaxCount);
	}

    private GetPositionsDelegate RandomPositions(int maxCount)
    {
        return delegate
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < _random.Next(maxCount); i++)
            {
                bool water;
                Vector3 position = RandomTreePosition(out water) + 
                    new Vector3( 
                            // (float) _random.NextDouble() * 0.2f, 
                            // 0, 
                            // (float) _random.NextDouble() * 0.2f
                            );
                //position.y = _heightmap[ (int)position.x, (int)position.z];
                if (!water)
                {
                    positions.Add(
                            position
                            );
                }
            }
            return positions;
        };
    }

    private Vector3 RandomTreePosition(out bool water)
    {
        water = false;
        var terrainPosition = _terrain.GetPosition();
        var treeRelativeX = Random.value % ChunkLength;
        var treeRelativeZ = Random.value % ChunkLength;
        var xRes = ((treeRelativeX / ChunkLength) * HeightmapResolution);
        var zRes = ((treeRelativeZ / ChunkLength) * HeightmapResolution);

        if (_heightmap[(int)(xRes), (int)(zRes)] < (SeaLevel / ChunkHeight))
        {
            water = true;
            return new Vector3();
        }

        var treeX = terrainPosition.x + treeRelativeX;
        var treeY = _terrainData.GetHeight((int)xRes, (int)zRes) - 0.5;
        // var treeY = _heightmap[(int)(treeRelativeZ * HeightmapResolution), (int)(treeRelativeX * HeightmapResolution)];
        var treeZ = terrainPosition.z + treeRelativeZ;
        return new Vector3(treeX, (float)treeY, treeZ);
    }

}
