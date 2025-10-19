using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Plays death music when player dies and stops it on scene reload
    /// Works with SceneReloadOnDeath system
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Death Music Controller")]
    public class DeathMusicController : MonoBehaviour
    {
        [Header("Audio Settings")]
        [Tooltip("The AudioClip to play when player dies")]
        public AudioClip deathMusic;
        
        [Tooltip("Volume of the death music (0-1)")]
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        
        [Tooltip("Loop the death music")]
        public bool loopMusic = false;
        
        [Header("Fade Settings")]
        [Tooltip("Fade in the death music")]
        public bool fadeIn = true;
        
        [Tooltip("Duration of fade in (seconds)")]
        public float fadeInDuration = 1f;
        
        [Tooltip("Fade out previous music when death music starts")]
        public bool fadeOutPreviousMusic = true;
        
        [Tooltip("Duration of fade out for previous music (seconds)")]
        public float fadeOutDuration = 0.5f;
        
        [Header("Player Detection")]
        [Tooltip("Player tag to find the player Health component")]
        public string playerTag = "Player";
        
        [Header("Audio Source")]
        [Tooltip("AudioSource to use (auto-creates if null)")]
        public AudioSource audioSource;
        
        [Tooltip("Create persistent AudioSource")]
        public bool createPersistentAudioSource = true;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private Health _playerHealth;
        private bool _musicStarted = false;
        private float _currentVolume = 0f;
        private float _targetVolume = 0f;
        private AudioSource _previousMusicSource;

        private void Start()
        {
            // Setup audio source
            SetupAudioSource();
            
            // Find and subscribe to player death
            SubscribeToPlayerHealth();
        }

        private void Update()
        {
            // Try to subscribe if player not found yet
            if (_playerHealth == null)
            {
                SubscribeToPlayerHealth();
            }
            
            // Handle fade in/out
            if (fadeIn && _musicStarted)
            {
                _currentVolume = Mathf.Lerp(_currentVolume, _targetVolume, Time.deltaTime / fadeInDuration);
                if (audioSource != null)
                {
                    audioSource.volume = _currentVolume;
                }
            }
        }

        /// <summary>
        /// Sets up the audio source
        /// </summary>
        private void SetupAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                
                if (audioSource == null && createPersistentAudioSource)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.loop = loopMusic;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log("DeathMusicController: Created AudioSource");
                    }
                }
            }

            if (audioSource != null)
            {
                audioSource.volume = 0f; // Start at 0 for fade in
                audioSource.loop = loopMusic;
            }
        }

        /// <summary>
        /// Finds player and subscribes to death event
        /// </summary>
        private void SubscribeToPlayerHealth()
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                _playerHealth = player.GetComponent<Health>();
                if (_playerHealth != null)
                {
                    _playerHealth.OnDeath += OnPlayerDeath;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"DeathMusicController: Subscribed to {player.name} Health.OnDeath");
                    }
                }
            }
        }

        /// <summary>
        /// Called when player dies - starts death music
        /// </summary>
        private void OnPlayerDeath()
        {
            if (_musicStarted) return; // Already playing
            
            if (deathMusic == null)
            {
                Debug.LogWarning("DeathMusicController: No death music assigned!");
                return;
            }

            if (audioSource == null)
            {
                Debug.LogError("DeathMusicController: No AudioSource available!");
                return;
            }

            if (showDebugInfo)
            {
                Debug.Log("DeathMusicController: Player died! Starting death music...");
            }

            // Fade out previous music if enabled
            if (fadeOutPreviousMusic)
            {
                FadeOutPreviousMusic();
            }

            // Start death music
            audioSource.clip = deathMusic;
            audioSource.Play();
            _musicStarted = true;
            
            if (fadeIn)
            {
                _currentVolume = 0f;
                _targetVolume = musicVolume;
                audioSource.volume = 0f;
            }
            else
            {
                audioSource.volume = musicVolume;
            }

            if (showDebugInfo)
            {
                Debug.Log($"DeathMusicController: Playing death music '{deathMusic.name}'");
            }
        }

        /// <summary>
        /// Fades out any background music that's playing
        /// </summary>
        private void FadeOutPreviousMusic()
        {
            // Try to find MMSoundManager or other music sources
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            
            foreach (AudioSource source in allAudioSources)
            {
                if (source != audioSource && source.isPlaying && source.clip != deathMusic)
                {
                    // Start fade out coroutine
                    StartCoroutine(FadeOutAudioSource(source, fadeOutDuration));
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"DeathMusicController: Fading out {source.name}");
                    }
                }
            }
        }

        /// <summary>
        /// Fades out an audio source
        /// </summary>
        private System.Collections.IEnumerator FadeOutAudioSource(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            
            source.volume = 0f;
            source.Stop();
        }

        /// <summary>
        /// Stops death music (called automatically on scene reload)
        /// </summary>
        private void OnDestroy()
        {
            // Cleanup subscription
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
            }
            
            // Stop music when scene reloads
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                
                if (showDebugInfo)
                {
                    Debug.Log("DeathMusicController: Stopped death music (scene reloading)");
                }
            }
        }

        /// <summary>
        /// Manually start death music (for testing)
        /// </summary>
        [ContextMenu("Test Death Music")]
        public void TestDeathMusic()
        {
            _musicStarted = false; // Reset flag
            OnPlayerDeath();
        }

        /// <summary>
        /// Manually stop death music
        /// </summary>
        [ContextMenu("Stop Death Music")]
        public void StopDeathMusic()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                _musicStarted = false;
                _currentVolume = 0f;
                
                if (showDebugInfo)
                {
                    Debug.Log("DeathMusicController: Music stopped manually");
                }
            }
        }
    }
}

