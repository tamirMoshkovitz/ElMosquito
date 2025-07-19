using System.Collections.Generic;
using _MSQT.Core.Scripts;
using UnityEngine;

namespace _MSQT.Audio.Scripts
{
    public class MSQTGenericPool<T>: MSQTMono where T: MSQTMono, IMSQTPoolable
    {
        [SerializeField] private int initialSize = 10;
        
        private List<T> _pool;
        public static MSQTGenericPool<T> Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                AddItemsToPool();
            }
            
            T poolable = _pool[0];
            _pool.RemoveAt(0);
            poolable.gameObject.SetActive(true);
            poolable.Reset();
            return poolable;
        }

        public void Return(T poolable)
        {
            poolable.gameObject.SetActive(false);
            _pool.Add(poolable);
        }
        
        private void AddItemsToPool()
        {
            for (int i = 0; i < initialSize; i++)
            {
                T clone = Instantiate(Resources.Load("Prefabs/" + typeof(T).Name)) as T;
                if (clone == null)
                {
                    Debug.LogError($"Failed to load prefab for {typeof(T).Name}. Make sure it exists in Resources/Prefabs.");
                    return;
                }
                clone.gameObject.SetActive(false);
                _pool.Add(clone);
            }
        }
        
    }
}