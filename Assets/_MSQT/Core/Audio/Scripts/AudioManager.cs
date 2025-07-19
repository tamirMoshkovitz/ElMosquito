using System.Collections.Generic;
using _MSQT.Core.Scripts;
using UnityEngine;

namespace _MSQT.Audio.Scripts
{
    // Singleton class for managing audio in the game
    public class AudioManager: MSQTMono
    {
        [Header("Background Music")]
        [SerializeField, Range(0f, 1f)] private float backgroundVolume = 0.5f;
        [SerializeField] private AudioClip backgroundMusic;
        private float _backgroundVolume;
        private AudioSource _backgroundMusicSource;
        private bool _shouldBackgroundMusicPlay = true;
        
        [Header("Master Volume")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        
    
        [Header("Sound Effects")]
        [SerializeField] private AudioClip[] audioClips;
        private Dictionary<string, AudioClip> _audioClipDictionary;

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        private bool _isMuted = false;
        private static AudioManager _instance;
        

        private void OnEnable()
        {
            GameEvents.PauseUnpauseBackgroundMusic += PauseUnpauseBackgroundMusic;
            GameEvents.MuteSounds += MuteSounds;
            _backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            _backgroundVolume = backgroundVolume; 
            if (backgroundMusic != null)
            {
                PlayBackgroundMusic(backgroundMusic);
            }
            else
            {
                Debug.LogWarning("No background music assigned.");
            }
        }

        private void OnDisable()
        {
            GameEvents.PauseUnpauseBackgroundMusic -= PauseUnpauseBackgroundMusic;
            GameEvents.MuteSounds -= MuteSounds;
            if (_backgroundMusicSource != null)
            {
                Destroy(_backgroundMusicSource);
            }
        }
        

        private void InitializeAudioClipDictionary()
        {
            _audioClipDictionary = new Dictionary<string, AudioClip>();
            foreach (var audioClip in audioClips)
            {
                _audioClipDictionary.TryAdd(audioClip.name, audioClip);
            }
        }
        

        private void MuteSounds()
        {
            _isMuted = !_isMuted;
            if (_isMuted)
            {
                SetBackgroundMusicVolume(0);
            }
            else
            {
                SetBackgroundMusicVolume(_backgroundVolume);
            }
        }


        private void PlayBackgroundMusic(AudioClip backgroundMusic)
        {
            _backgroundMusicSource.Stop();
            _backgroundMusicSource.clip = backgroundMusic;
            _backgroundMusicSource.volume = backgroundVolume;
            _backgroundMusicSource.loop = true;
            _backgroundMusicSource.Play();
            // _isBackgroundMusicPlaying = true;
        }
    
        private void SetBackgroundMusicVolume(float volume)
        {
            _backgroundMusicSource.volume = volume;
        }

        public void PlaySound(Vector3 position, string soundName, float volume = 1f, float pitch = 1f, bool loop = false,
            float spatialBlend = 0f)
        {
            var audioSource = AudioSourcePool.Instance.Get();
            if (audioSource != null)
            {
                if(!GetAudioClip(soundName)) return;
                audioSource.transform.position = position;
                audioSource.SetAudioClip(GetAudioClip(soundName));
                audioSource.SetVolume(volume);
                audioSource.SetPitch(pitch);
                audioSource.SetLoop(loop);
                audioSource.SetSpatialBlend(spatialBlend);
                audioSource.Play();
            }
        }

        public void StopSound(string soundName)
        {
            GameEvents.StopSoundByName?.Invoke(soundName);
        }
        
        
        private AudioClip GetAudioClip(string soundName)
        {
            if (_audioClipDictionary.TryGetValue(soundName, out var audioClip))
            {
                return audioClip;
            }

            Debug.Log("Sound not found: " + soundName);
            return null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_backgroundMusicSource == null) return;
            if (hasFocus)
            {
                if (_shouldBackgroundMusicPlay)
                {
                    _backgroundMusicSource.UnPause();
                }
            }
            else
            {
                _backgroundMusicSource.Pause();
            }
        }

        private void PauseUnpauseBackgroundMusic()
        {
                if (_backgroundMusicSource.isPlaying)
                {
                    _backgroundMusicSource.Pause();
                    _shouldBackgroundMusicPlay = false;
                }
                else
                {
                    _backgroundMusicSource.UnPause();
                    _shouldBackgroundMusicPlay = true;
                }

        }
        public void StopBackgroundMusicPlaying()
        {
            _backgroundMusicSource.Stop();
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            AudioListener.volume = masterVolume;
        }
        
        public float GetMasterVolume()
        {
            return masterVolume;
        }
    }
}