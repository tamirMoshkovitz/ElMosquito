namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class SpeedDecorator: AbstractPowerUpDecorator
    {
        public SpeedDecorator(IMosquitoDecorator previousDecorator) : base(previousDecorator) { }
        
        public override float GetMovementSpeed()
        {
            return PreviousDecorator.GetMovementSpeed() * 1.2f;
        }

        public override IMosquitoDecorator GetPreviousDecorator()
        {
            return PreviousDecorator.GetPreviousDecorator(); // Speed decorators come after maneuver decorators and are not independent
        }
    }
}