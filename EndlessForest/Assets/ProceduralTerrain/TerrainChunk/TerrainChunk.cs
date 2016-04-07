using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralTerrain;
using UnityEditorInternal;
using Assets.ProceduralTerrain.Utils;

public class TerrainChunk : MonoBehaviour
{
    public int HeightmapResolution;
    public float ChunkLength;
    public float ChunkHeight;
    public int TreesMaxCount;
    public int BushesMaxCount;
    public Material Material;

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    private GameObjectPool _treePool;
    private GameObjectPool _bushesPool;

    private List<GameObject> _activeTrees;
    private List<GameObject> _activeBushes;

    private TerrainData _terrainData;
    private float[,] _heightmap;
    private Terrain _terrain;
    private System.Random _random;
    private bool _reloading;
    private bool _initialized;

    public void Awake()
    {
        _treePool = GameObject.FindGameObjectWithTag("TreePool").GetComponent<GameObjectPool>();
        _bushesPool = GameObject.FindGameObjectWithTag("BushesPool").GetComponent<GameObjectPool>();
        _activeTrees = new List<GameObject>();
        _activeBushes = new List<GameObject>();
        _heightmap = new float[HeightmapResolution, HeightmapResolution];
        _random = new System.Random();

        CreateHeightmap();
        CreateTerrainData();
        _terrain = Terrain.CreateTerrainGameObject(_terrainData).GetComponent<Terrain>();
        _terrain.Flush();
        InitTextures();
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
        _terrainData.heightmapResolution = (int) (Mathf.Pow(2, HeightmapResolution) + 1);
        _terrainData.alphamapResolution = (int)(Mathf.Pow(2, HeightmapResolution) + 1);
        _terrainData.size = new Vector3(ChunkLength, ChunkHeight, ChunkLength);
        _terrainData.SetHeights(0, 0, _heightmap);
    }

    private void InitTextures()
    {
        _terrain.materialTemplate = Material;
        _terrainData.wavingGrassAmount = 2;
        _terrainData.wavingGrassStrength = 1;
        _terrain.materialType = Terrain.MaterialType.Custom;
    }

    /////////////////////// ACTIVATE ///////////////////////

    public void Activate()
    {
        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        _reloading = true;
        yield return StartCoroutine(ActivateTerrain());
        yield return StartCoroutine(ActivateTrees());
        yield return StartCoroutine(ActivateBushes());
        FadeIn();
        _reloading = false;
    }

    private IEnumerator ActivateTerrain()
    {
        _terrain.gameObject.transform.position = new Vector3(ChunkX * ChunkLength, 0, ChunkZ * ChunkLength);
        CreateHeightmap();
        _terrainData.SetHeights(0, 0, _heightmap);
        _terrain.gameObject.SetActive(true);

        _reloading = false;
        yield return null;
    }

    private IEnumerator ActivateTrees()
    {
        foreach (Vector3 position in RandomTreePositions())
        {
            GameObject tree = _treePool.Take();
            tree.transform.position = position;
            FadeUtils.SetFadeOut(tree);
            tree.SetActive(true);
            _activeTrees.Add(tree);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ActivateBushes()
    {
        foreach (var position in RandomBushesPositions())
        {
            GameObject bush = _bushesPool.Take();
            bush.transform.position = position;
            bush.SetActive(true);
            _activeBushes.Add(bush);
            yield return null;
        }
    }

    /////////////////////// DEACTIVATE ///////////////////////

    private void Deactivate()
    {
        DeactivateTerrain();
        DeactivateTrees();
        DeactivateBushes();
    }

    private void DeactivateTerrain()
    {
        if (_terrain != null)
        {
            _terrain.gameObject.SetActive(false);
        }
    }

    private void DeactivateTrees()
    {
        foreach (var tree in _activeTrees)
        {
            if (tree != null)
            {
                tree.SetActive(false);
                _treePool.Put(tree);
            }
        }
        _activeTrees.Clear();
    }

    private void DeactivateBushes()
    {
        foreach (var bush in _activeBushes)
        {
            if (bush != null)
            {
                bush.SetActive(false);
                _bushesPool.Put(bush);
            }
        }
        _activeTrees.Clear();
    }

    public void FadeIn()
    {
        var objectsToFade = _activeTrees.Concat(_activeBushes).ToList();
        StartCoroutine(FadeUtils.FadeIn(objectsToFade));
    }

    public IEnumerator FadeOut()
    {
        var objectsToFade = _activeTrees.Concat(_activeBushes).ToList();
        yield return StartCoroutine(FadeUtils.FadeOut(objectsToFade));
    }

    /////////////////////////////// OTHER ///////////////////////////////

    private void CreateHeightmap()
    {
        for (var zRes = 0; zRes < HeightmapResolution; zRes++)
        {
            for (var xRes = 0; xRes < HeightmapResolution; xRes++)
            {
                var xCoordinate = ChunkX + xRes /( HeightmapResolution - 1f);
                var zCoordinate = ChunkZ + zRes / ( HeightmapResolution - 1f);
                _heightmap[zRes, xRes] = Mathf.PerlinNoise(xCoordinate, zCoordinate);
            }
        }
    }

    private IEnumerable<Vector3> RandomBushesPositions()
    {
        for (int i = 0; i < _random.Next(TreesMaxCount); i++)
        {
            yield return RandomTreePosition();
        }
    }

    private IEnumerable<Vector3> RandomTreePositions()
    {
        for (int i = 0; i < _random.Next(TreesMaxCount); i++)
        {
            yield return RandomTreePosition();
        }
    }


    private Vector3 RandomTreePosition()
    {
        var terrainPosition = _terrain.GetPosition();
        var treeRelativeX = Random.value % ChunkLength;
        var treeRelativeZ = Random.value % ChunkLength;
        var xRes = ((ChunkX + treeRelativeX) * HeightmapResolution);
        var zRes = ((ChunkZ + treeRelativeZ) * HeightmapResolution);

        var treeX = terrainPosition.x + treeRelativeX * ChunkLength;
        var treeY = _terrainData.GetHeight((int)xRes, (int)zRes) - 0.1;
        var treeZ = terrainPosition.z + treeRelativeZ * ChunkLength;
        return new Vector3(treeX, (float)treeY, treeZ);
    }

}
