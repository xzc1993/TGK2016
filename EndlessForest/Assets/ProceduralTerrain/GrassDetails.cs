using System;
using UnityEngine;

namespace Assets.ProceduralTerrain
{
    [Serializable]
    public class GrassDetails
    {
        public Texture2D GrassTexture;
        [Range(0, 1)]
        public float Probability;
        public Color HealthyColor;
        public Color DryColor;
        public Color WavingGrassTint;
    }
}