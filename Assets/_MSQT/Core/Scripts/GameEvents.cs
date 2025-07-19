using System;

namespace _MSQT.Core.Scripts
{
    public static class GameEvents
    {
        public static Action LostPowerUp;

        #region Audio Events

        public static Action PauseUnpauseBackgroundMusic;
        public static Action MuteSounds;
        public static Action<String> StopSoundByName;
        
        #endregion
    }
}