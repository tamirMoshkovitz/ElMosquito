using System;
using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts;
using _MSQT.Player.Scripts.MosquitoBehaviors;
using _MSQT.Player.Scripts.MosquitoDecorators;
using DG.Tweening;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public abstract class MSQTPowerUps<T> : MSQTMono, IMSQTPoolable where T : IMosquitoDecorator
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
                PowerUpsSpawner.Instance.ReturnPowerUp(this.transform);
            }
        }

        private void OnEnable()
        {
            Vector3 originalScale = transform.localScale;
            transform.localScale = Vector3.zero;

            // Scale up to original scale over 0.3 seconds, then punch
            transform.DOScale(originalScale, 0.3f)
                .OnComplete(() =>
                {
                    // Add punch effect
                    transform.DOPunchScale(originalScale * 0.2f, 0.3f, 10, 1);
                });
        }
    }
}