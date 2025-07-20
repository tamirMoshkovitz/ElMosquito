using _MSQT.Player.Scripts.MosquitoDecorators;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class ManeuverPowerUp : MSQTPowerUps<ManeuverDecorator>
    {
        private void Awake()
        {
            SetConstructor(previousDecorator => new ManeuverDecorator(previousDecorator));
        }
    }
}