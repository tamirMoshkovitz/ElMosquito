namespace _MSQT.Player.Scripts.MosquitoBehaviors
{
    /// <summary>
    /// Decorator that enhances the mosquito's rotation speed
    /// </summary>
    public class ManeuverPowerUp: IMosquitoDecorator
    {
        private readonly IMosquitoDecorator _previousBehaviour;
        
        public ManeuverPowerUp(IMosquitoDecorator mosquitoBehaviour)
        {
            _previousBehaviour = mosquitoBehaviour;
        }
        public float GetMovementSpeed()
        {
            return _previousBehaviour.GetMovementSpeed();
        }

        public float GetRotationSpeed()
        {
            return _previousBehaviour.GetRotationSpeed() * 1.5f; // Increase rotation speed by 50%
        }

        public float GetHealth()
        {
            return _previousBehaviour.GetHealth();
        }

        public float GetDamage()
        {
            return _previousBehaviour.GetDamage();
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return _previousBehaviour;
        }
    }
}