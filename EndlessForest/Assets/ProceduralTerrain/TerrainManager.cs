using UnityEngine;
using Wasabimole.ProceduralTree;

public class TerrainManager : MonoBehaviour
{
    public Material Material;
    [Range(5, 10)]
    public ProceduralTree ProceduralTreePrefab;
    public int HeightmapResolution;
    public float ChunkLength;
    public float ChunkHeight;
   

    // Use this for initialization
    void Start ()
	{
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j <3; j++)
            {
                TerrainChunk terrainChunk = new TerrainChunk()
                {
                    ChunkX = i,
                    ChunkZ = j,
                    HeightmapResolution = (int) Mathf.Pow(2, HeightmapResolution) + 1,
                    ChunkHeight = ChunkHeight,
                    ChunkLength = ChunkLength,
                    Material = Material,
                    TreePrefab = ProceduralTreePrefab
                };
                terrainChunk.Render();
            }
        }
    }

    // Update is called once per frame
    void Update () {
	    
	}
}
