using _MSQT.Core.Scripts;
using UnityEngine;

namespace _MSQT.Enemy.Scripts
{
    public class BossLife: MSQTMono
    {
        
        [SerializeField] private RectTransform healthBar;
        private float _health = 100f;
        private Vector2 _barSize;

        private void Awake()
        {
            _barSize = healthBar.sizeDelta;
        }

        private void Update()
        {
            if (!healthBar || !healthBar.gameObject || !healthBar.gameObject.activeInHierarchy)
                return;
            
            float clampedHealth = Mathf.Clamp01(_health / 100f);
            healthBar.sizeDelta = new Vector2(clampedHealth * _barSize.x, healthBar.sizeDelta.y);
        }
        
        public void GetHurt(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                SceneLoader.LoadScene(SceneName.WinEnding);
            }
        }

    }
}