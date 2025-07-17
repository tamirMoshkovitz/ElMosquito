using _MSQT.Player.Scripts.MosquitoDecorators;

namespace _MSQT.Player.Scripts.MosquitoBehaviors
{
    public class SpeedDecorator: IMosquitoDecorator
    {
        private readonly IMosquitoDecorator _previousDecorator;
        public SpeedDecorator(IMosquitoDecorator previousDecorator)
        {
            _previousDecorator = previousDecorator;
        }
        
        public float GetMovementSpeed()
        {
            return _previousDecorator.GetMovementSpeed() * 1.2f;
        }

        public float GetRotationSpeed()
        {
            return _previousDecorator.GetRotationSpeed();
        }

        public float GetDamage()
        {
            return _previousDecorator.GetDamage();
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return _previousDecorator.GetPreviousDecorator(); // Speed decorators come after maneuver decorators and are not independent
        }

        public float UpdateHP(float deltaTime)
        {
            return _previousDecorator.UpdateHP(deltaTime);
        }

        public float GetHealingSpeed()
        {
            return _previousDecorator.GetHealingSpeed();
        }
    }
}