using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProceduralTerrain.Utils
{
    public static class FadeUtils
    {
        private static float seconds = 1f;
        private static int counter = 50;

        public static IEnumerator FadeIn(List<GameObject> objects)
        {
            for (int i = 0; i < counter; i++)
            {
                foreach (var gameObject in objects)
                {
                    var renderer = gameObject.GetComponent<Renderer>();
                    var color = renderer.material.color;
                    color.a = 1.0f * i / counter;
                    renderer.material.color = color;
                }
                yield return new WaitForSeconds(seconds / counter);
            }
        }

        public static IEnumerator FadeOut(List<GameObject> objects)
        {
            for (int i = 0; i < counter; i++)
            {
                foreach (var gameObject in objects)
                {
                    var renderer = gameObject.GetComponent<Renderer>();
                    var color = renderer.material.color;
                    color.a = 1.0f - (float)i / counter;
                    renderer.material.color = color;
                }
                yield return new WaitForSeconds(seconds / counter);
            }
        }

        public static void SetFadeOut(GameObject obj)
        {
            var renderer = obj.GetComponent<Renderer>();
            var color = renderer.material.color;
            color.a = 0f;
            renderer.material.color = color;
        }
    }
}