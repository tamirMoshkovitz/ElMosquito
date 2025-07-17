using _MSQT.Player.Scripts.MosquitoBehaviors;

namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class BasicMosquitoBehavior: IMosquitoDecorator
    {
        private protected float MovementSpeed;
        private protected float RotationSpeed;
        private protected float Damage;
        private protected float HealingSpeed = 1f;
        
        public BasicMosquitoBehavior(float movementSpeed, float rotationSpeed, float damage)
        {
            MovementSpeed = movementSpeed;
            RotationSpeed = rotationSpeed;
            Damage = damage;
        }

        public virtual float GetMovementSpeed()
        {
            return MovementSpeed;
        }
        
        public virtual float GetRotationSpeed()
        {
            return RotationSpeed;
        }
        
        public virtual float GetDamage()
        {
            return Damage;
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