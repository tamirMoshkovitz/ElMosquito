using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _MSQT.Player.Scripts.UI
{
    [Serializable]
    public class PlayerInfoManager
    {
        [SerializeField] private PlayerInfoBar hpBar;
        [SerializeField] private PlayerInfoBar maneuverBar;
        [SerializeField] private PlayerInfoBar damageBar;
        private bool _hpChanged;
        private bool _maneuverChanged;
        private bool _damageChanged;
        
        public void Awake()
        {
            hpBar.Awake();
            maneuverBar.Awake();
            damageBar.Awake();
        }

        public void Start()
        {
            hpBar.UpdateBarNoLerp();
            maneuverBar.UpdateBarNoLerp();
            damageBar.UpdateBarNoLerp();
        }

        public void SetHP(float value)
        {
            _hpChanged = hpBar.SetValue(value);
        }

        public void SetManeuver(float value)
        {
            _maneuverChanged = maneuverBar.SetValue(value);
        }

        public void SetDamage(float value)
        {
            _damageChanged = damageBar.SetValue(value);
        }
        
        private void OnValidate()
        {
            if (hpBar == null)
                Debug.LogError($"{nameof(hpBar)} is not assigned in PlayerInfoManager.");
            if (maneuverBar == null)
                Debug.LogError($"{nameof(maneuverBar)} is not assigned in PlayerInfoManager.");
            if (damageBar == null)
                Debug.LogError($"{nameof(damageBar)} is not assigned in PlayerInfoManager.");
        }
        
        public void UpdateHPNoLerp(float value)
        {
            hpBar.AddToBarNoLerp(value);
        }
    }
}
