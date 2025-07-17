using System;
using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts;
using _MSQT.Player.Scripts.MosquitoBehaviors;
using _MSQT.Player.Scripts.MosquitoDecorators;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public abstract class _MSQTPowerUps<T> : MSQTMono, IMSQTPoolable where T : IMosquitoDecorator
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
                PlayerControler player = other.gameObject.GetComponent<PlayerControler>();
                player.MosquitoBehaviour = _constructorFunc(player.MosquitoBehaviour);
                // TODO: Return to pool
            }
        }
    }
}