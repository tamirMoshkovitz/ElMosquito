using System;
using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts;
using _MSQT.Player.Scripts.MosquitoDecorators;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public abstract class MSQTPowerUps<T> : MSQTPowerUpBaseClass where T : IMosquitoDecorator
    {
        private Func<IMosquitoDecorator, T> _constructorFunc;

        protected void SetConstructor(Func<IMosquitoDecorator, T> constructor)
        {
            _constructorFunc = constructor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _constructorFunc != null)
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                player.MosquitoBehaviour = _constructorFunc(player.MosquitoBehaviour);
                PowerUpsSpawner.Instance.ReturnPowerUp(this.transform);
            }
        }
    }
}