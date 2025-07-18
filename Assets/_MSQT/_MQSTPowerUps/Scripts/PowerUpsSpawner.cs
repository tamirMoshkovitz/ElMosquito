using System;
using System.Collections.Generic;
using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts.MosquitoDecorators;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class PowerUpsSpawner : MSQTMono
    {
        [Serializable]
        private class PowerUpData
        {
            [SerializeField] private Transform powerUpPrefab;
            [SerializeField] private int maxCount;
            public int MaxCount { get => maxCount; }
            public Transform Prefab { get => powerUpPrefab; }
        }
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 30f;
        [SerializeField] private int maxManeuverPowerUps = 3;
        [SerializeField] private int maxDamagePowerUps = 5;
        [SerializeField] private int maxHealthPowerUps = 4;
        
        [Header("PowerUps List")]
        [SerializeField] private List<PowerUpData> prefabAndAmount = new ();

        [Header("Train Bounds")]
        [SerializeField] private Transform trainFront;
        [SerializeField] private Transform trainBack;
        [SerializeField] private Transform trainCeiling;
        [SerializeField] private Transform trainFloor;
        [SerializeField] private Transform trainLeft;
        [SerializeField] private Transform trainRight;

        public static PowerUpsSpawner Instance;
        private float _timer;
        private List<Transform> _pool = new List<Transform>();
        private Stack<Transform> _collectedPowerUps = new Stack<Transform>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                foreach (var prefab in prefabAndAmount)
                {
                    for (int i = 0; i < prefab.MaxCount; i++)
                    {
                        GameObject powerUpObject = Instantiate(prefab.Prefab.gameObject, Vector3.zero, Quaternion.identity);
                        powerUpObject.SetActive(false);
                        powerUpObject.transform.SetParent(transform);
                        if (i == 0) // the game starts with one power up of each type on the player
                            _collectedPowerUps.Push(powerUpObject.transform);
                        else
                            _pool.Add(powerUpObject.transform);
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvents.LostPowerUp += OnLostPowerUp;
        }
        
        private void OnDisable()
        {
            GameEvents.LostPowerUp -= OnLostPowerUp;
        }

        private void Update()
        {
            // Spawn power-ups at regular intervals
            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                _timer = 0f;
                SpawnPowerUp();
            }
        }

        public void SpawnPowerUp()
        {
            if (_pool.Count <= 0) return;
            
            int index = Random.Range(0, _pool.Count);
            if (0 <= index && index < _pool.Count && _pool[index] != null)
            {
                Transform powerUp = _pool[index];
                _pool.RemoveAt(index);
                SetRandomPosition(powerUp);
                powerUp.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("PowerUp is null, returning null.");
            }
        }


        public void ReturnPowerUp(Transform powerUp)
        {
            if (powerUp != null)
            {
                powerUp.gameObject.GetComponent<MSQTPowerUpBaseClass>().DisablePowerUp();
                _collectedPowerUps.Push(powerUp);
            }
            else
            {
                Debug.LogWarning("Attempted to return a null PowerUp.");
            }
        }
        
        private void SetRandomPosition(Transform powerUp)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(trainBack.position.x, trainFront.position.x),
                Random.Range(trainFloor.position.y, trainCeiling.position.y),
                Random.Range(trainLeft.position.z, trainRight.position.z)
            );

            powerUp.position = randomPosition;
        }

        private void OnLostPowerUp()
        {
            if (_collectedPowerUps.Count > 0)
                _pool.Add(_collectedPowerUps.Pop());
        }
    }
}