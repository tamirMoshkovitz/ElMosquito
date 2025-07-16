namespace _MSQT.Player.Scripts.MosquitoBehaviors
{
    public interface IMosquitoDecorator
    {
        float GetMovementSpeed();
        float GetRotationSpeed();
        float GetHealth();
        float GetDamage();
        IMosquitoDecorator GetPreviousDecorator();
    }
}