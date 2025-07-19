using UnityEngine;

namespace _MSQT.Core.Audio.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraSizeAdjuster : MonoBehaviour
    {
        private const float TargetAspect = 1920f / 1080f; // FullHD

        private Camera _cam;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            UpdateViewport();
        }

        void Update()
        {
            if (Mathf.Abs((float)Screen.width / Screen.height - TargetAspect) > 0.01f)
            {
                UpdateViewport();
            }
        }

        private void UpdateViewport()
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float scaleHeight = screenAspect / TargetAspect;

            if (scaleHeight < 1.0f)
            {
                // Add letterbox (top/bottom bars)
                Rect rect = _cam.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;

                _cam.rect = rect;
            }
            else
            {
                // Add pillarbox (left/right bars)
                float scaleWidth = 1.0f / scaleHeight;

                Rect rect = _cam.rect;

                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;

                _cam.rect = rect;
            }

            Debug.Log($"Updated camera viewport to maintain aspect ratio: {TargetAspect:F2}");
        }
    }
}