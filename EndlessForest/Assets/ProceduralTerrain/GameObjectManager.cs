using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ProceduralTerrain.Utils;
using UnityEngine;

namespace Assets.ProceduralTerrain
{
    public delegate List<Vector3> GetPositionsDelegate();

    public class GameObjectManager
    {
        public List<GameObject> ObjectList { get; set; }
        public GameObjectPool ObjectPool { get; set; }
        public GetPositionsDelegate GetPositions { get; set; }
        private bool _fade;

        public GameObjectManager(GameObjectPool objectPool, GetPositionsDelegate getPositions, bool fade=true)
        {
            this._fade = fade;
            ObjectList = new List<GameObject>();
            ObjectPool = objectPool;
            GetPositions = getPositions;
        }

        public IEnumerator Activate()
        {
            System.Random random = new System.Random();
            foreach (Vector3 position in GetPositions())
            {
                GameObject obj = ObjectPool.Take();
                obj.transform.position = position;
                if (ObjectPool.tag == "RockPool")
                {
                    RaycastHit hit;
                    var ray = new Ray(position, Vector3.up);
                    if (Physics.Raycast(ray, out hit, 2.0f))
                    {
                        obj.transform.up = hit.normal + new Vector3(
                                (float)(random.NextDouble() - 0.5) * 0.2f,
                                (float)(random.NextDouble() - 0.5) * 0.2f,
                                (float)(random.NextDouble() - 0.5) * 0.2f);
                    }
                }
                if (_fade)
                {
                    FadeUtils.SetFadeOut(obj);
                }
                obj.SetActive(true);
                ObjectList.Add(obj);
                yield return null;
            }
        }

        public void Deactivate()
        {
            foreach (var obj in ObjectList)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    ObjectPool.Put(obj);
                }
            }
            ObjectList.Clear();
        }

    }
}
