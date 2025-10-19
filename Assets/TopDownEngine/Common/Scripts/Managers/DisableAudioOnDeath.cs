using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Disables an AudioSource when the player dies
    /// Simple and direct - just turns off the AudioSource component
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Disable Audio On Death")]
    public class DisableAudioOnDeath : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("The AudioSource to disable when player dies")]
        public AudioSource TargetAudioSource;

        [Header("Player Settings")]
        [Tooltip("The player tag to listen for death events")]
        public string PlayerTag = "Player";

        [Header("Optional - Also Stop")]
        [Tooltip("If true, also call Stop() before disabling (recommended)")]
        public bool AlsoStopAudio = true;

        // Private variables
        private Health _playerHealth;
        private bool _hasDisabled = false;

        /// <summary>
        /// On start, find audio source and player
        /// </summary>
        protected virtual void Start()
        {
            // Find audio source if not assigned
            if (TargetAudioSource == null)
            {
                TargetAudioSource = GetComponent<AudioSource>();
            }

            if (TargetAudioSource == null)
            {
                Debug.LogWarning("DisableAudioOnDeath: No AudioSource assigned or found! This script will not work.");
                enabled = false;
                return;
            }

            Debug.Log($"DisableAudioOnDeath: AudioSource found on '{TargetAudioSource.gameObject.name}'");

            // Find and subscribe to player
            SubscribeToPlayer();
        }

        /// <summary>
        /// Find player and subscribe to death event
        /// </summary>
        protected virtual void SubscribeToPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
            
            if (player != null)
            {
                _playerHealth = player.GetComponent<Health>();
                
                if (_playerHealth != null)
                {
                    _playerHealth.OnDeath += OnPlayerDeath;
                    Debug.Log($"DisableAudioOnDeath: Subscribed to player '{player.name}' death event");
                }
                else
                {
                    Debug.LogWarning($"DisableAudioOnDeath: Player '{player.name}' found but has no Health component!");
                }
            }
            else
            {
                Debug.LogWarning($"DisableAudioOnDeath: No player found with tag '{PlayerTag}'!");
            }
        }

        /// <summary>
        /// Called when the player dies
        /// </summary>
        protected virtual void OnPlayerDeath()
        {
            if (_hasDisabled || TargetAudioSource == null)
            {
                return;
            }

            Debug.Log($"DisableAudioOnDeath: Player died! Disabling AudioSource on '{TargetAudioSource.gameObject.name}'");

            // Stop the audio first (recommended)
            if (AlsoStopAudio)
            {
                TargetAudioSource.Stop();
                Debug.Log($"DisableAudioOnDeath: Audio stopped");
            }

            // Disable the AudioSource component
            TargetAudioSource.enabled = false;
            Debug.Log($"DisableAudioOnDeath: AudioSource disabled");

            _hasDisabled = true;
        }

        /// <summary>
        /// Unsubscribe when destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
            }
        }

        /// <summary>
        /// Public method to manually disable the audio
        /// </summary>
        public virtual void DisableAudio()
        {
            if (TargetAudioSource != null && !_hasDisabled)
            {
                if (AlsoStopAudio)
                {
                    TargetAudioSource.Stop();
                }
                TargetAudioSource.enabled = false;
                _hasDisabled = true;
                Debug.Log($"DisableAudioOnDeath: Audio manually disabled");
            }
        }

        /// <summary>
        /// Public method to re-enable the audio (for testing/restarting)
        /// </summary>
        public virtual void EnableAudio()
        {
            if (TargetAudioSource != null)
            {
                TargetAudioSource.enabled = true;
                _hasDisabled = false;
                Debug.Log($"DisableAudioOnDeath: Audio re-enabled");
            }
        }
    }
}

