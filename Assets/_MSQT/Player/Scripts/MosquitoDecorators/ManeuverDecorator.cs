namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    /// <summary>
    /// Decorator that enhances the mosquito's rotation speed
    /// </summary>
    public class ManeuverDecorator: AbstractPowerUpDecorator
    {
        public static readonly float ManeuverIncreaseParameter = 1.2f;
        
        public ManeuverDecorator(IMosquitoDecorator mosquitoBehaviour) : base(mosquitoBehaviour) { }

        public override float GetRotationSpeed()
        {
            return PreviousDecorator.GetRotationSpeed() * ManeuverIncreaseParameter; // Increase rotation speed by 50%
        }
    }
}