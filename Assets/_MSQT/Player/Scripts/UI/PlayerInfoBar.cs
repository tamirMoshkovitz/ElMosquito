using System;
using System.Collections;
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
        public bool TrySetValue(float newValue)
        {
            float difference = newValue - _targetValue;
            _targetValue = newValue;
            _startWidth = bar ? bar.sizeDelta.x : 0f;
            return difference != 0;
        }
        
        public float GetValue()
        {
            return _targetValue;
        }
        
        public void Start()
        {
            if (bar)
            {
                float targetWidth = Mathf.Clamp(_targetValue * _fullWidth, 0f, _fullWidth);
                bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);
            }
        }
        
        public void UpdateBar(float value)
        {
            float clampedValue = Mathf.Clamp01(value);

            _targetValue = clampedValue;
            float targetWidth = _targetValue * _fullWidth;
            bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);
            _startWidth = targetWidth;
        }

        public IEnumerator UpdateLerp(float value) // TODO: first time it lerps from 1 to value instead of from initial value
        {
            if (Mathf.Approximately(value, _targetValue)) yield break;
            
            float timer = 0;
            float targetWidth = Mathf.Clamp(value * _fullWidth, 0f, _fullWidth);
            while (timer < lerpDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / lerpDuration);
                float newWidth = Mathf.Lerp(_startWidth, targetWidth, t);
                bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);
                yield return null;
            }
            bar.sizeDelta = new Vector2(targetWidth, bar.sizeDelta.y);
            _startWidth = targetWidth;
            _targetValue = value;
        }
    }
}