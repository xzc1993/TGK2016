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


    class GameObjectManager
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
            foreach (Vector3 position in GetPositions())
            {
                GameObject obj = ObjectPool.Take();
                obj.transform.position = position;
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
