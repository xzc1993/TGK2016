using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Assets.ProceduralTerrain
{
    public class GameObjectPool : MonoBehaviour
    {
        public int PoolSize;
        public List<GameObject> Prefabs;

        private Random _random = new Random();
        private Queue<GameObject> _inactiveObjects;

        public void Start()
        {
            _inactiveObjects = new Queue<GameObject>(PoolSize);
            for (int i = 0; i < PoolSize; i++)
            {
                var obj = Instantiate(Prefabs[_random.Next(Prefabs.Count)]);
                obj.SetActive(false);
                Put(obj);
            }
        }

        public void Put(GameObject obj)
        {
            _inactiveObjects.Enqueue(obj);
        }

        public GameObject Take()
        {
            return _inactiveObjects.Dequeue();
        }

        public T Take<T>()
        {
            return _inactiveObjects.Dequeue().GetComponent<T>();
        }

    }
}
