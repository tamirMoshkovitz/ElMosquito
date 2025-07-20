namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public class DamageDecorator: AbstractPowerUpDecorator
    {
        public static readonly float DamageIncreaseParameter = 1.5f;
        public DamageDecorator(IMosquitoDecorator previousDecorator) : base(previousDecorator) { }

        public override float GetDamage()
        {
            return PreviousDecorator.GetDamage() * DamageIncreaseParameter; // Increase damage by 50%
        }
    }
}