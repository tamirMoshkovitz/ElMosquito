namespace _MSQT.Player.Scripts.MosquitoDecorators
{
    public interface IMosquitoDecorator
    {
        float GetMovementSpeed();
        float GetRotationSpeed();
        float GetDamage();
        float GetHealingSpeed();
        float UpdateHP(float deltaTime);
        IMosquitoDecorator GetPreviousDecorator();
    }
}