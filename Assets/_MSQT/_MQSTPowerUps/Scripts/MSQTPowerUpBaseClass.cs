using _MSQT.Core.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class MSQTPowerUpBaseClass: MSQTMono
    {
        private void OnEnable()
        {
            Vector3 originalScale = transform.localScale;
            transform.localScale = Vector3.zero;

            transform.DOScale(originalScale, 0.3f)
                .OnComplete(() =>
                {
                    transform.DOPunchScale(originalScale * 0.2f, 0.3f);
                });
        }

        public void DisablePowerUp()
        {
            Vector3 originalScale = transform.localScale;
            transform.DOPunchScale(Vector3.one * 1.2f, 0.15f).OnComplete(
                () => transform.DOScale(Vector3.zero, 0.15f).OnComplete(
                    () =>
                    {
                        transform.localScale = originalScale;
                        gameObject.SetActive(false);
                    }
                )
            );
        }
    }
}