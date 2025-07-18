using _MSQT.Player.Scripts.MosquitoBehaviors;

namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class DamageDecorator: IMosquitoDecorator
    {
        private readonly IMosquitoDecorator _previousDecorator;
        public DamageDecorator(IMosquitoDecorator previousDecorator)
        {
            _previousDecorator = previousDecorator;
        }
        
        public float GetMovementSpeed()
        {
            return _previousDecorator.GetMovementSpeed();
        }

        public float GetRotationSpeed()
        {
            return _previousDecorator.GetRotationSpeed();
        }

        public float GetDamage()
        {
            return _previousDecorator.GetDamage() * 1.5f; // Increase damage by 50%
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return _previousDecorator;
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