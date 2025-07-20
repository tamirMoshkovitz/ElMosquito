using _MSQT.Player.Scripts.MosquitoBehaviors;

namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class FasterHealingDecorator: IMosquitoDecorator
    {
        private readonly IMosquitoDecorator _previousDecorator;
        private float _healingSpeed;
        
        public FasterHealingDecorator(IMosquitoDecorator previousDecorator)
        {
            _previousDecorator = previousDecorator;
            _healingSpeed = previousDecorator.GetHealingSpeed() + 1f;
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
            return _previousDecorator.GetDamage();
        }

        public PlayerController GetPlayer()
        {
            throw new System.NotImplementedException();
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return _previousDecorator;
        }

        public float UpdateHP(float deltaTime)
        {
            return _previousDecorator.UpdateHP(deltaTime) * _healingSpeed;
        }

        public float GetHealingSpeed()
        {
            return _healingSpeed;
        }
    }
}