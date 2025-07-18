using System;
using System.Collections.Generic;
using _MSQT.Core.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class PowerUpsPool : MSQTMono
    {
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 30f;
        
        [Header("PowerUps List")]
        [SerializeField] private List<Transform> pool = new List<Transform>();

        [Header("Train Bounds")]
        [SerializeField] private Transform trainFront;
        [SerializeField] private Transform trainBack;
        [SerializeField] private Transform trainCeiling;
        [SerializeField] private Transform trainFloor;
        [SerializeField] private Transform trainLeft;
        [SerializeField] private Transform trainRight;

        public static PowerUpsPool Instance;
        private float _timer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
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
            if (pool.Count <= 0) return;
            
            int index = Random.Range(0, pool.Count);
            
            if (pool[index] != null)
            {
                Transform powerUp = pool[index];
                pool.RemoveAt(index);
                
                Vector3 randomPosition = new Vector3(
                    Random.Range(trainBack.position.x, trainFront.position.x),
                    Random.Range(trainFloor.position.y, trainCeiling.position.y),
                    Random.Range(trainLeft.position.z, trainRight.position.z)
                );
                
                powerUp.position = randomPosition;
                
                powerUp.gameObject.SetActive(true);
                return;
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
                powerUp.gameObject.SetActive(false);
                pool.Add(powerUp);
            }
            else
            {
                Debug.LogWarning("Attempted to return a null PowerUp.");
            }
        }
    }
}