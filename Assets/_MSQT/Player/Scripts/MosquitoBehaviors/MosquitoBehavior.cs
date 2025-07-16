namespace _MSQT.Player.Scripts.MosquitoBehaviors
{
    public class BasicMosquitoBehavior: IMosquitoDecorator
    {
        private protected float MovementSpeed;
        private protected float RotationSpeed;
        private protected float Health;
        private protected float Damage;
        
        public BasicMosquitoBehavior(float movementSpeed, float rotationSpeed, float health, float damage)
        {
            MovementSpeed = movementSpeed;
            RotationSpeed = rotationSpeed;
            Health = health;
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
        
        public virtual float GetHealth()
        {
            return Health;
        }
        
        public virtual float GetDamage()
        {
            return Damage;
        }

        public IMosquitoDecorator GetPreviousDecorator()
        {
            return this;
        }
    }
}