namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class FasterHealingDecorator: AbstractPowerUpDecorator
    {
        private readonly float _healingSpeed;
        
        public FasterHealingDecorator(IMosquitoDecorator previousDecorator) : base(previousDecorator)
        {
            _healingSpeed = previousDecorator.GetHealingSpeed() + 1f;
        }

        public override float UpdateHP(float deltaTime)
        {
            return PreviousDecorator.UpdateHP(deltaTime) * _healingSpeed;
        }

        public override float GetHealingSpeed()
        {
            return _healingSpeed;
        }
    }
}