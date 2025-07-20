namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public abstract class AbstractPowerUpDecorator: IMosquitoDecorator
    {
        protected readonly IMosquitoDecorator PreviousDecorator;
        
        public AbstractPowerUpDecorator(IMosquitoDecorator previousDecorator)
        {
            PreviousDecorator = previousDecorator;
        }
        
        public virtual float GetMovementSpeed()
        {
            return PreviousDecorator.GetMovementSpeed();
        }

        public virtual float GetRotationSpeed()
        {
            return PreviousDecorator.GetRotationSpeed();
        }

        public virtual float GetDamage()
        {
            return PreviousDecorator.GetDamage();
        }

        public virtual float GetHealingSpeed()
        {
            return PreviousDecorator.GetHealingSpeed();
        }

        public virtual float UpdateHP(float deltaTime)
        {
            return PreviousDecorator.UpdateHP(deltaTime);
        }

        public virtual IMosquitoDecorator GetPreviousDecorator()
        {
            return PreviousDecorator;
        }
    }
}