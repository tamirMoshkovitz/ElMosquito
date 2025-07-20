using _MSQT.Player.Scripts.MosquitoDecorators;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class HealPowerUp: MSQTPowerUps<FasterHealingDecorator>
    {
        private void Awake()
        {
            SetConstructor(previousDecorator => new FasterHealingDecorator(previousDecorator));
        }
    }
}