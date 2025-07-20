using _MSQT.Player.Scripts.MosquitoDecorators;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class DamagePowerUp: MSQTPowerUps<DamageDecorator>
    {
        private void Awake()
        {
            SetConstructor(previousDecorator => new DamageDecorator(previousDecorator));
        }
    }
}