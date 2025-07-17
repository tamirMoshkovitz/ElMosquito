
using _MSQT.Player.Scripts.MosquitoBehaviors;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class ManeuverPowerUp : _MSQTPowerUps<ManeuverDecorator>
    {
        private void Awake()
        {
            SetConstructor(previousDecorator => new ManeuverDecorator(previousDecorator));
        }
    }
}