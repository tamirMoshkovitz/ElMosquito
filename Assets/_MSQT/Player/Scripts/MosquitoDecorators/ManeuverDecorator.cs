using _MSQT.Player.Scripts.MosquitoDecorators;

namespace _MSQT.Player.Scripts.MosquitoBehaviors
{
    /// <summary>
    /// Decorator that enhances the mosquito's rotation speed
    /// </summary>
    public class ManeuverDecorator: IMosquitoDecorator
    {
        private readonly IMosquitoDecorator _previousBehaviour;
        
        public ManeuverDecorator(IMosquitoDecorator mosquitoBehaviour)
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

        public float GetDamage()
        {
            return _previousBehaviour.GetDamage();
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return _previousBehaviour;
        }

        public float UpdateHP(float deltaTime)
        {
            return _previousBehaviour.UpdateHP(deltaTime);
        }
        
        public float GetHealingSpeed()
        {
            return _previousBehaviour.GetHealingSpeed();
        }
    }
}