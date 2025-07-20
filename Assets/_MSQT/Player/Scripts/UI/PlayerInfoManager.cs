using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
        
        public void Awake()
        {
            hpBar.Awake();
            maneuverBar.Awake();
            damageBar.Awake();
        }

        public void Start()
        {
            hpBar.Start();
            maneuverBar.Start();
            damageBar.Start();
        }

        public void SetHP(float value)
        {
            hpBar.TrySetValue(value);
        }

        public void SetManeuver(float value)
        {
            maneuverBar.TrySetValue(value);
        }

        public void SetDamage(float value)
        {
            damageBar.TrySetValue(value);
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
        
        public void UpdateHPBar(float value)
        {
            hpBar.UpdateBar(value);
        }
        
        public IEnumerator UpdateHpBarWithLerp(float value)
        {
            return hpBar.UpdateLerp(value);
        }

        public IEnumerator UpdateDamageBar(float value)
        {
            return damageBar.UpdateLerp(value);
        }
        
        public IEnumerator UpdateManeuverBar(float value)
        {
            return maneuverBar.UpdateLerp(value);
        }
    }
}
