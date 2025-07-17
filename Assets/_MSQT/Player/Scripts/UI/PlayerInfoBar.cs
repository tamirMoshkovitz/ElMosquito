using System;
using UnityEngine;

namespace _MSQT.Player.Scripts.UI
{
    [Serializable]
    public class PlayerInfoBar
    {
        [SerializeField] private RectTransform bar;
        [SerializeField] float lerpDuration = 0.5f;

        private float _fullWidth;
        private float _targetValue = 0f;
        private float _lerpTimer = 0f;
        private float _startWidth = 0f;
        
        
        public void Awake()
        {
            _fullWidth = bar.rect.width;
        }

        /// <summary>
        /// Set a new target value for the bar.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>true if the new value is set (happends only if it is significant enough)</returns>
        public bool SetValue(float newValue)
        {
            float difference = newValue - _targetValue;
            _targetValue = newValue;
            _lerpTimer = 0f;
            _startWidth = bar ? bar.sizeDelta.x : 0f;
            return difference != 0;
        }
        
        public float GetValue()
        {
            return _targetValue;
        }
        
        public void UpdateBarNoLerp()
        {
            if (bar)
            {
                float targetWidth = Mathf.Clamp(_targetValue * _fullWidth, 0f, _fullWidth);
                bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);
            }
        }
        
        public void AddToBarNoLerp(float value)
        {
            float clampedValue = Mathf.Clamp01(value);

            _targetValue = clampedValue;
            float targetWidth = _targetValue * _fullWidth;
            bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);
            _startWidth = targetWidth;
        }

        // public void UpdateBar(float deltaTime) // called in update by the PlayerInfoManager
        // {
        //     if (bar)
        //     {
        //         float targetWidth = Mathf.Clamp(_targetValue * _fullWidth, 0f, _fullWidth);
        //
        //         _lerpTimer += deltaTime;
        //         float t = lerpDuration > 0 ? Mathf.Clamp01(_lerpTimer / lerpDuration) : 1f;
        //
        //         float newWidth = Mathf.Lerp(_startWidth, targetWidth, t);
        //         bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);
        //
        //         if (t >= 1f)
        //         {
        //             _lerpTimer = 0f;
        //         }
        //     }
        // }
    }
}