using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Stops music when the player dies
    /// Attach this to a GameObject with an AudioSource (or reference one)
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Music Stop On Death")]
    public class MusicStopOnDeath : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("The AudioSource to stop when player dies (if null, will search on this GameObject)")]
        public AudioSource MusicAudioSource;

        [Header("Player Settings")]
        [Tooltip("The player tag to listen for death events")]
        public string PlayerTag = "Player";

        [Header("Fade Settings")]
        [Tooltip("If true, fade out the music instead of stopping instantly")]
        public bool FadeOut = true;

        [Tooltip("Duration of the fade out in seconds")]
        public float FadeOutDuration = 1f;

        // Private variables
        private Health _playerHealth;
        private bool _isFading = false;
        private float _fadeTimer = 0f;
        private float _originalVolume = 1f;

        /// <summary>
        /// On start, find the audio source and player
        /// </summary>
        protected virtual void Start()
        {
            // Find audio source if not assigned
            if (MusicAudioSource == null)
            {
                MusicAudioSource = GetComponent<AudioSource>();
                if (MusicAudioSource == null)
                {
                    Debug.LogWarning("MusicStopOnDeath: No AudioSource found! Please assign one in the Inspector.");
                    enabled = false;
                    return;
                }
            }

            // Store original volume
            _originalVolume = MusicAudioSource.volume;

            // Find the player
            FindPlayer();
        }

        /// <summary>
        /// Find and subscribe to player death event
        /// </summary>
        protected virtual void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
            
            if (player != null)
            {
                _playerHealth = player.GetComponent<Health>();
                
                if (_playerHealth != null)
                {
                    // Subscribe to death event
                    _playerHealth.OnDeath += OnPlayerDeath;
                    Debug.Log($"MusicStopOnDeath: Subscribed to player death event");
                }
                else
                {
                    Debug.LogWarning($"MusicStopOnDeath: Player found but has no Health component!");
                }
            }
            else
            {
                Debug.LogWarning($"MusicStopOnDeath: No player found with tag '{PlayerTag}'!");
            }
        }

        /// <summary>
        /// Called when the player dies
        /// </summary>
        protected virtual void OnPlayerDeath()
        {
            if (MusicAudioSource == null)
            {
                return;
            }

            if (FadeOut)
            {
                // Start fade out
                _isFading = true;
                _fadeTimer = 0f;
                Debug.Log("MusicStopOnDeath: Fading out music...");
            }
            else
            {
                // Stop immediately
                MusicAudioSource.Stop();
                Debug.Log("MusicStopOnDeath: Music stopped immediately");
            }
        }

        /// <summary>
        /// Update to handle fade out
        /// </summary>
        protected virtual void Update()
        {
            if (_isFading && MusicAudioSource != null)
            {
                _fadeTimer += Time.unscaledDeltaTime; // Use unscaled time in case game is paused

                // Calculate fade
                float fadeProgress = _fadeTimer / FadeOutDuration;
                MusicAudioSource.volume = Mathf.Lerp(_originalVolume, 0f, fadeProgress);

                // Stop when fade is complete
                if (fadeProgress >= 1f)
                {
                    MusicAudioSource.Stop();
                    MusicAudioSource.volume = _originalVolume; // Reset volume for next play
                    _isFading = false;
                    Debug.Log("MusicStopOnDeath: Music fade complete");
                }
            }
        }

        /// <summary>
        /// Unsubscribe from events when destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
            }
        }

        /// <summary>
        /// Public method to manually stop the music
        /// </summary>
        public virtual void StopMusic()
        {
            if (MusicAudioSource != null)
            {
                MusicAudioSource.Stop();
                _isFading = false;
            }
        }

        /// <summary>
        /// Public method to manually start fading out
        /// </summary>
        public virtual void FadeOutMusic()
        {
            if (MusicAudioSource != null && !_isFading)
            {
                _isFading = true;
                _fadeTimer = 0f;
                _originalVolume = MusicAudioSource.volume;
            }
        }
    }
}

