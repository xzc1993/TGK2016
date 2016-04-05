using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wasabimole.ProceduralTree;

public class TerrainChunkElementsPool
{
    public ProceduralTree TreePrefab { get; set; }

    private readonly Queue<ProceduralTree> _inactiveTrees;

    public TerrainChunkElementsPool(int treesPoolSize, ProceduralTree treePrefab)
    {
        TreePrefab = treePrefab;
        _inactiveTrees = new Queue<ProceduralTree>(treesPoolSize);
        for (int i = 0; i < treesPoolSize; i++)
        {
            ProceduralTree tree = CreateTree();
            putTree(tree);
        }
    }

    public void putTree(ProceduralTree tree)
    {
        _inactiveTrees.Enqueue(tree);
    }

    public ProceduralTree takeTree()
    {
        if (_inactiveTrees.Count == 0)
        {
            ProceduralTree tree = CreateTree();
            putTree(tree);
        }
        return _inactiveTrees.Dequeue();
    }

    private ProceduralTree CreateTree()
    {
        UnityEngine.Random.seed = Random.Range(0, 100000000);
        TreePrefab.Seed = UnityEngine.Random.seed;
        ProceduralTree tree = (ProceduralTree)GameObject.Instantiate(TreePrefab, Vector3.zero, Quaternion.identity);
        tree.name = "Tree " + tree.Seed;
        tree.gameObject.SetActive(false);
        return tree;
    }
}
