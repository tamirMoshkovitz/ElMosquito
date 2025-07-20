namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class BasicMosquitoBehavior: IMosquitoDecorator
    {
        private readonly float _movementSpeed;
        private readonly float _rotationSpeed;
        private readonly float _damage;
        private const float HealingSpeed = .5f;

        public BasicMosquitoBehavior(float movementSpeed, float rotationSpeed, float damage)
        {
            _movementSpeed = movementSpeed;
            _rotationSpeed = rotationSpeed;
            _damage = damage;
        }

        public virtual float GetMovementSpeed()
        {
            return _movementSpeed;
        }
        
        public virtual float GetRotationSpeed()
        {
            return _rotationSpeed;
        }
        
        public virtual float GetDamage()
        {
            return _damage;
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return this;
        }

        public float UpdateHP(float deltaTime)
        {
            return (HealingSpeed * deltaTime);
        }

        public float GetHealingSpeed()
        {
            return HealingSpeed;
        }
    }
}