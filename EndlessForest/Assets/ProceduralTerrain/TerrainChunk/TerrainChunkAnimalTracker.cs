using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using Assets.ProceduralTerrain;
using UnityEditorInternal;
using Assets.ProceduralTerrain.Utils;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using Random = UnityEngine.Random;

public class TerrainChunkAnimalTracker : MonoBehaviour
{
	private TerrainChunk chunk;

	public TerrainChunkAnimalTracker()
	{
		
	}

	public void setHomeChunk(TerrainChunk chunk){
		this.chunk = chunk;
	}

	public TerrainChunk getHomeChunk(){
		return chunk;
	}


	public void OnAnimalInterChunkMovement(GameObject animal, GameObject oldChunk){
		if (oldChunk != null) {
			oldChunk.GetComponent<TerrainChunkAnimalTracker>().getHomeChunk().getVelociraptorManager().ObjectList.Remove( animal);
			this.chunk.getVelociraptorManager().ObjectList.Add( animal);
		}
	}
}


