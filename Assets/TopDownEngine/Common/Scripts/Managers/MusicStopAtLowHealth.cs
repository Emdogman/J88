using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Plays music and stops it when player health reaches a threshold (default: 1)
    /// Monitors player health every frame for instant response
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Music Stop At Low Health")]
    public class MusicStopAtLowHealth : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("The AudioSource to play and control")]
        public AudioSource MusicAudioSource;

        [Header("Player Settings")]
        [Tooltip("The player tag to find and monitor")]
        public string PlayerTag = "Player";

        [Header("Music Settings")]
        [Tooltip("If true, automatically play the music on Start")]
        public bool PlayOnStart = true;

        [Tooltip("If true, stop music when player health is low")]
        public bool StopOnLowHealth = true;

        [Tooltip("Health value at or below which music stops")]
        public float HealthThreshold = 1f;

        // Private variables
        private Health _playerHealth;
        private bool _hasStopped = false;

        /// <summary>
        /// On start, find audio source, player, and optionally play music
        /// </summary>
        protected virtual void Start()
        {
            // Find audio source if not assigned
            if (MusicAudioSource == null)
            {
                MusicAudioSource = GetComponent<AudioSource>();
            }

            if (MusicAudioSource == null)
            {
                Debug.LogWarning("MusicStopAtLowHealth: No AudioSource assigned or found!");
                enabled = false;
                return;
            }

            // Find player
            GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
            
            if (player != null)
            {
                _playerHealth = player.GetComponent<Health>();
                
                if (_playerHealth != null)
                {
                    Debug.Log($"MusicStopAtLowHealth: Found player '{player.name}' with Health component");
                }
                else
                {
                    Debug.LogWarning($"MusicStopAtLowHealth: Player found but has no Health component!");
                }
            }
            else
            {
                Debug.LogWarning($"MusicStopAtLowHealth: No player found with tag '{PlayerTag}'!");
            }

            // Play music if requested
            if (PlayOnStart && MusicAudioSource.clip != null)
            {
                MusicAudioSource.Play();
                Debug.Log($"MusicStopAtLowHealth: Music started - '{MusicAudioSource.clip.name}'");
            }
        }

        /// <summary>
        /// Check player health every frame
        /// </summary>
        protected virtual void Update()
        {
            if (!StopOnLowHealth || _hasStopped || _playerHealth == null || MusicAudioSource == null)
            {
                return;
            }

            // Check if health has dropped to threshold
            if (_playerHealth.CurrentHealth <= HealthThreshold)
            {
                Debug.Log($"MusicStopAtLowHealth: Player health at {_playerHealth.CurrentHealth}! Stopping music");
                StopMusic();
            }
        }

        /// <summary>
        /// Stop the music and disable the AudioSource
        /// </summary>
        protected virtual void StopMusic()
        {
            if (MusicAudioSource == null || _hasStopped)
            {
                return;
            }

            // Stop the audio
            MusicAudioSource.Stop();
            
            // Disable the AudioSource component
            MusicAudioSource.enabled = false;
            
            _hasStopped = true;
            
            Debug.Log("MusicStopAtLowHealth: Music stopped and AudioSource disabled");
        }

        /// <summary>
        /// Public method to manually stop the music
        /// </summary>
        public virtual void ManualStopMusic()
        {
            StopMusic();
        }

        /// <summary>
        /// Public method to restart the music (for testing/restarting level)
        /// </summary>
        public virtual void RestartMusic()
        {
            if (MusicAudioSource == null)
            {
                return;
            }

            MusicAudioSource.enabled = true;
            MusicAudioSource.Play();
            _hasStopped = false;
            
            Debug.Log("MusicStopAtLowHealth: Music restarted");
        }
    }
}

